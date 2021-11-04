from pyswip import Prolog
from math import nan, pow
import sys
import os
import pathlib
import re

def main():
    path = sys.argv[1]
    path = path.replace("\\","/")

    hypPath = path + "/hyp.pl"
    testPath = path[0:path.rfind("/")] + "/test"

    prolog = Prolog()
    prolog.consult(testPath + "/bk.pl")
    prolog.consult(hypPath)
    
    with open(testPath + "/output.txt") as f:
        testResults = f.read().splitlines()
    
    actual = list(map(stringToBool, testResults))

    with open(testPath + "/input.txt") as f:
        tests = f.read().splitlines()

    prediction = []
    for test in tests:
        testResult = bool(list(prolog.query(test)))
        prediction.append(testResult)

    tp = 0
    fp = 0
    tn = 0
    fn = 0

    for a, p in zip(actual, prediction):
        if (a == True and p == True):
            tp += 1
        elif (a == True and p == False):
            fn += 1
        elif (a == False and p == True):
            fp += 1
        else:
            tn += 1

    beta = 2

    if (tp + fp == 0):
        precision = 0
    else:
        precision = tp / (tp + fp)

    if (tp + fn == 0):
        recall = 0
    else:
        recall = tp / (tp + fn)
    # if (beta**2 * precision) + recall == 0:
    #     f_score = nan
    # else:
    #     f_score = (1 + beta**2) * ((precision * recall) / ((beta**2 * precision) + recall))

    # print(actual)
    # print(prediction)
    # print("tp: %d  fp: %d" % (tp, fp))
    # print("fn: %d  tn: %d" % (fn, tn))
    # print("precision: %f" % (precision))
    # print("recall: %f" % (recall))
    # print("f_score: %f" % (f_score))

    max_clauses, max_body, max_vars = getConstraints(path)

    stats_file = open(path + "/temp2.csv", 'w')
    stats_file.write("%s,%s,%s,%d,%d,%d,%d,%f,%f" % (max_vars, max_clauses, max_body, tp, tn, fp, fn, recall, precision))
    stats_file.close()


def stringToBool(s):
    return s == "true"

def getConstraints(path):
    f = open(path + "/bias.pl", "r")
    max_clauses = getNumFromConstraint(f.readline())
    max_body = getNumFromConstraint(f.readline())
    max_vars = getNumFromConstraint(f.readline())
    return max_clauses, max_body, max_vars

def getNumFromConstraint(constraint_string):
    return re.findall("\d", constraint_string)[0]


if __name__ == "__main__":
    main()