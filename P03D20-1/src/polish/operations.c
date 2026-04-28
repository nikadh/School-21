#include "operations.h"

int is_unary(char operation) {
    return operation == 'S' || operation == 'C' || operation == 'T' || operation == 'G' || operation == 'Q' ||
           operation == 'L';
}

int is_multiplication_or_division(char operation) { return operation == '*' || operation == '/'; }

int is_addition_or_subtraction(char operation) { return operation == '+' || operation == '-'; }

int is_binary(char operation) {
    return is_multiplication_or_division(operation) || is_addition_or_subtraction(operation);
}

int is_operator(char operation) { return is_unary(operation) || is_binary(operation); }

int get_priority(char operation) {
    return is_unary(operation) ? 0 : (is_multiplication_or_division(operation) ? 1 : 2);
}
