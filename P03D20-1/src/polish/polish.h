#ifndef POLISH_H
#define POLISH_H

#define POLISH_TYPE_ROOT 0;
#define POLISH_TYPE_OPERATOR 1;
#define POLISH_TYPE_OPERAND 2;
#define POLISH_TYPE_X 3;

struct polish {
    struct polish* next;
    int type;
    char operation;
    double value;
};

struct polish* init_polish(int type, char operation, double value);
struct polish* push_polish(struct polish* root, int type, char operation, double value);
void destroy_polish(struct polish* root);

#endif
