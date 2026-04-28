#ifndef OPERATIONS_H
#define OPERATIONS_H

int is_unary(char operation);
int is_multiplication_or_division(char operation);
int is_addition_or_subtraction(char operation);
int is_binary(char operation);
int is_operator(char operation);
int get_priority(char operation);

#endif
