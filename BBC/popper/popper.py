#!/usr/bin/env python3

import logging
import sys
import timeit
from popper.util import Settings, Stats, timeout, parse_settings, format_program, parse_args
from popper.asp import ClingoGrounder, ClingoSolver
from popper.tester import Tester
from popper.constrain import Constrain
from popper.generate import generate_program
from popper.core import Grounding, Clause

num_of_hyps = 1
start_time = timeit.default_timer()

class Outcome:
    ALL = 'all'
    SOME = 'some'
    NONE = 'none'

class Con:
    GENERALISATION = 'generalisation'
    SPECIALISATION = 'specialisation'
    REDUNDANCY = 'redundancy'
    BANISH = 'banish'

OUTCOME_TO_CONSTRAINTS = {
        (Outcome.ALL, Outcome.NONE)  : (Con.BANISH,),
        (Outcome.ALL, Outcome.SOME)  : (Con.GENERALISATION,),
        (Outcome.SOME, Outcome.NONE) : (Con.SPECIALISATION,),
        (Outcome.SOME, Outcome.SOME) : (Con.SPECIALISATION, Con.GENERALISATION),
        (Outcome.NONE, Outcome.NONE) : (Con.SPECIALISATION, Con.REDUNDANCY),
        (Outcome.NONE, Outcome.SOME) : (Con.SPECIALISATION, Con.REDUNDANCY, Con.GENERALISATION)
    }

def ground_rules(stats, grounder, max_clauses, max_vars, clauses):
    out = set()
    for clause in clauses:
        head, body = clause
        # find bindings for variables in the constraint
        assignments = grounder.find_bindings(clause, max_clauses, max_vars)

        # keep only standard literals
        body = tuple(literal for literal in body if not literal.meta)

        # ground the clause for each variable assignment
        for assignment in assignments:
            out.add(Grounding.ground_clause((head, body), assignment))
    
    stats.register_ground_rules(out)

    return out

def decide_outcome(conf_matrix):
    tp, fn, tn, fp = conf_matrix
    if fn == 0:
        positive_outcome = Outcome.ALL # complete
    elif tp == 0 and fn > 0:
        positive_outcome = Outcome.NONE # totally incomplete
    else:
        positive_outcome = Outcome.SOME # incomplete

    if fp == 0:
        negative_outcome = Outcome.NONE  # consistent
    # elif FP == self.num_neg:     # AC: this line may not work with minimal testing
        # negative_outcome = Outcome.ALL # totally inconsistent
    else:
        negative_outcome = Outcome.SOME # inconsistent

    return (positive_outcome, negative_outcome)

def build_rules(settings, stats, constrainer, tester, program, before, min_clause, outcome):
    (positive_outcome, negative_outcome) = outcome
    # RM: If you don't use these two lines you need another three entries in the OUTCOME_TO_CONSTRAINTS table (one for every positive outcome combined with negative outcome ALL).
    if negative_outcome == Outcome.ALL:
         negative_outcome = Outcome.SOME

    rules = set()
    for constraint_type in OUTCOME_TO_CONSTRAINTS[(positive_outcome, negative_outcome)]:
        if constraint_type == Con.GENERALISATION:
            rules.update(constrainer.generalisation_constraint(program, before, min_clause))
        elif constraint_type == Con.SPECIALISATION:
            rules.update(constrainer.specialisation_constraint(program, before, min_clause))
        elif constraint_type == Con.REDUNDANCY:
            rules.update(constrainer.redundancy_constraint(program, before, min_clause))
        elif constraint_type == Con.BANISH:
            rules.update(constrainer.banish_constraint(program, before, min_clause))

    if settings.functional_test and tester.is_non_functional(program):
        rules.update(constrainer.generalisation_constraint(program, before, min_clause))

    # eliminate generalisations of clauses that contain redundant literals
    for rule in tester.check_redundant_literal(program):
        rules.update(constrainer.redundant_literal_constraint(rule, before, min_clause))

    # eliminate generalisations of programs that contain redundant clauses
    if tester.check_redundant_clause(program):
        rules.update(constrainer.generalisation_constraint(program, before, min_clause))

    if len(program) > 1:
        # evaluate inconsistent sub-clauses
        for rule in program:
            if Clause.is_separable(rule) and tester.is_inconsistent(rule):
                for x in constrainer.generalisation_constraint([rule], before, min_clause):
                    rules.add(x)

        # eliminate totally incomplete rules
        if all(Clause.is_separable(rule) for rule in program):
            for rule in program:
                if tester.is_totally_incomplete(rule):
                    for x in constrainer.redundancy_constraint([rule], before, min_clause):
                        rules.add(x)

    stats.register_rules(rules)

    return rules

PROG_KEY = 'prog'

def calc_score(conf_matrix):
    # tp, fn, tn, fp = conf_matrix
    # return tp + tn

    # beta = parse_args().beta
    # tp, fn, tn, fp = conf_matrix
    # if (tp + fp == 0 or tp + fn == 0):
    #     return 0
    # precision = tp / (tp + fp)
    # recall = tp / (tp + fn)
    # if ((((beta**2 * precision) + recall)) == 0):
    #     return 0
    # f_score = (1 + beta**2) * ((precision * recall) / ((beta**2 * precision) + recall))
    # return f_score

    beta = parse_args().beta
    tp, fn, tn, fp = conf_matrix

    if (tp + fn + fp == 0):
        return 0
    else:
        return ((1 + beta**2) * tp) / ((1 + beta**2) * tp + beta**2 * fn + fp)


def popper(settings, stats):
    solver = ClingoSolver(settings)
    tester = Tester(settings)
    settings.num_pos, settings.num_neg = len(tester.pos), len(tester.neg)
    grounder = ClingoGrounder()
    constrainer = Constrain()
    best_score = None

    for size in range(1, settings.max_literals + 1):
        stats.update_num_literals(size)
        solver.update_number_of_literals(size)

        while True:
            # GENERATE HYPOTHESIS
            with stats.duration('generate'):
                model = solver.get_model()
                if not model:
                    break
                (program, before, min_clause) = generate_program(model)

            # TEST HYPOTHESIS
            with stats.duration('test'):
                conf_matrix = tester.test(program)
                outcome = decide_outcome(conf_matrix)
                score = calc_score(conf_matrix)

            stats.register_program(program, conf_matrix)

            # UPDATE BEST PROGRAM
            if best_score == None or score > best_score:
                best_score = score

                prog_stats = stats.make_program_stats(program, conf_matrix)

                global num_of_hyps
                write_hypothesis(prog_stats.code, conf_matrix, num_of_hyps)
                num_of_hyps += 1

                if outcome == (Outcome.ALL, Outcome.NONE):
                    stats.register_solution(program, conf_matrix)
                    return stats.solution.code

                stats.register_best_program(program, conf_matrix)

            # BUILD RULES
            with stats.duration('build'):
                rules = build_rules(settings, stats, constrainer, tester, program, before, min_clause, outcome)

            # GROUND RULES
            with stats.duration('ground'):
                rules = ground_rules(stats, grounder, solver.max_clauses, solver.max_vars, rules)

            # UPDATE SOLVER
            with stats.duration('add'):
                solver.add_ground_clauses(rules)

    stats.register_completion()
    return stats.best_program.code if stats.best_program else None

def show_hspace(settings):
    f = lambda i, m: print(f'% program {i}\n{format_program(generate_program(m)[0])}')
    ClingoSolver.get_hspace(settings, f)

def learn_solution(settings):
    log_level = logging.DEBUG if settings.debug else logging.INFO
    logging.basicConfig(level=log_level, stream=sys.stderr, format='%(message)s')
    stats = Stats(log_best_programs=settings.info)
    timeout(popper, (settings, stats), timeout_duration=int(settings.timeout))
    stats.log_final_result()
    if settings.stats:
        stats.show()


def write_hypothesis(hypothesis, conf_matrix, hyp_num):
    # Get time elapsed
    global start_time
    time_elapsed = timeit.default_timer() - start_time

    # Write hyp file
    parser_args = parse_args()
    f = open("%s/hyp%d.pl" % (parser_args.kbpath, hyp_num), "w")
    f.write(hypothesis)
    f.close()

    # Write data file
    tp, fn, tn, fp = conf_matrix
    if (tp + fp == 0):
        precision = -1
    else:
        precision = tp / (tp + fp)
    if (tp + fn == 0):
        recall = -1
    else:
        recall = tp / (tp + fn)

    hypothesis = hypothesis.replace("\n", " | ")
    
    f = open("%s/temp1_%d.csv" % (parser_args.kbpath, hyp_num), "w")
    f.write("%d;%d;%d;%d;%d;%f;%f;%s;%f" % (parser_args.beta, tp, fn, tn, fp, precision, recall, hypothesis, time_elapsed))
    f.close()

if __name__ == '__main__':
    settings = parse_settings()
    if settings.hspace:
        show_hspace(settings)
    else:
        learn_solution(settings)