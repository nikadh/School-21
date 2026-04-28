#include "polish.h"

#include <stdio.h>
#include <stdlib.h>

struct polish* init_polish(int type, char operation, double value) {
    struct polish* polish = calloc(1, sizeof(struct polish));
    polish->type = type;
    polish->operation = operation;
    polish->value = value;
    polish->next = NULL;

    return polish;
}

struct polish* push_polish(struct polish* root, int type, char operation, double value) {
    struct polish* current = root;
    while (NULL != current->next) {
        current = current->next;
    }

    struct polish* node = init_polish(type, operation, value);
    current->next = node;

    return node;
}

void destroy_polish(struct polish* root) {
    struct polish* current = root;
    while (NULL != current) {
        struct polish* temp = current;
        current = current->next;
        free(temp);
    }
}
