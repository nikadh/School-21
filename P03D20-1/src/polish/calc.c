#include "calc.h"

#include <math.h>
#include <stdio.h>

#include "operations.h"

double calc(struct polish* polish, double x) {
    int count = 0;
    double list[1024] = {0};

    struct polish* current = polish->next;
    while (NULL != current) {
        if (1 == current->type) {
            if (is_unary(current->operation)) {
                list[count - 1] = calc_unary(list[count - 1] /*, current->operation*/);
            } else {
                list[count - 2] = calc_binary(list[count - 2], list[count - 1], current->operation);
                count--;
            }
        } else if (2 == current->type) {
            list[count] = current->value;
            count++;
        } else {
            list[count] = x;
            count++;
        }

        current = current->next;
    }

    return list[0];
}

double calc_unary(double n /*, char operation*/) { return n; }

double calc_binary(double a, double b, char operation) {
    double result;
    switch (operation) {
        case '+':
            result = a + b;
            break;
        case '-':
            result = a - b;
            break;
        case '*':
            result = a * b;
            break;
        case '/':
            result = a / b;
            break;
    }
    return result;
}
