#ifndef CALC_H
#define CALC_H

#include "polish.h"

double calc(struct polish* polish, double x);
double calc_unary(double n /*, char operation*/);
double calc_binary(double a, double b, char operation);

#endif
