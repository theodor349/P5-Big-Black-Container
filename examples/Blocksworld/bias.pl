%% python3 popper.py examples/alleven/
%% likes(A) :- empty(A).
%% likes(A) :- head(A,C),even(C),tail(A,B),likes(B).
%% 0.62s user 0.03s system 99% cpu 0.648 total

max_clauses(5).
max_vars(5).
max_body(5).
enable_pi.

head_pred(movebtob, 3).
body_pred(on, 2).
body_pred(ontable, 1).
body_pred(clear, 1).
