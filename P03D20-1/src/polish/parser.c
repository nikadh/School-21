#include "parser.h"

#include <stdio.h>
#include <stdlib.h>

#include "operations.h"
#include "polish.h"

struct parser_node* init_parser_node(int is_root, char operation) {
    struct parser_node* node = calloc(1, sizeof(struct parser_node));
    node->next = NULL;
    node->prev = NULL;
    node->is_root = is_root;
    node->operation = operation;

    return node;
}

struct parser_node* push_parser_node(struct parser_node* root, char operation) {
    struct parser_node* current = root;
    while (NULL != current->next) {
        current = current->next;
    }

    struct parser_node* node = init_parser_node(0, operation);
    current->next = node;
    node->prev = current;

    return node;
}

struct parser_node* get_last_parser_node(struct parser_node* root) {
    struct parser_node* current = root;
    while (NULL != current->next) {
        current = current->next;
    }

    return current;
}

void remove_last_parser_node(struct parser_node* root) {
    struct parser_node* current = root;
    struct parser_node* previous = NULL;
    while (NULL != current->next) {
        previous = current;
        current = current->next;
    }
    free(current);
    if (NULL != previous) {
        previous->next = NULL;
    }
}

void destroy(struct parser_node* root) {
    if (NULL != root->next) {
        destroy(root->next);
    }
    free(root);
}

struct polish* parse(char* source) {
    struct polish* list = init_polish(0, '\0', 0.0);
    struct parser_node* parser_node = init_parser_node(1, '\0');

    for (; '\0' != *source; source++) {
        char symbol = *source;

        if (is_digit(symbol)) {
            double value;
            int shift;
            (void)parse_double(source, &value, &shift);
            (void)push_polish(list, 2, '\0', value);
            source = source + shift - 1;
        } else if (is_operator(symbol)) {
            int op_priority = get_priority(symbol);

            struct parser_node* last_parser_node = get_last_parser_node(parser_node);

            while (last_parser_node->is_root == 0 && is_operator(last_parser_node->operation) &&
                   get_priority(last_parser_node->operation) <= op_priority) {
                if (is_operator(last_parser_node->operation) &&
                    get_priority(last_parser_node->operation) <= op_priority) {
                    push_polish(list, 1, last_parser_node->operation, 0.0);
                    last_parser_node = last_parser_node->prev;
                    remove_last_parser_node(parser_node);
                }
            }
            (void)push_parser_node(parser_node, symbol);
        } else if (symbol == '(') {
            push_parser_node(parser_node, symbol);
        } else if (symbol == ')') {
            struct parser_node* last_parser_node = get_last_parser_node(parser_node);
            while (last_parser_node->operation == '(' || is_operator(last_parser_node->operation)) {
                if (is_operator(last_parser_node->operation)) {
                    push_polish(list, 1, last_parser_node->operation, 0.0);
                }
                last_parser_node = last_parser_node->prev;
                remove_last_parser_node(parser_node);
            }
            last_parser_node = last_parser_node->prev;
            if (last_parser_node->is_root == 0 && is_unary(last_parser_node->operation)) {
                push_polish(list, 1, last_parser_node->operation, 0.0);
            }
        } else if (symbol == 'X') {
            push_polish(list, 3, '\0', 0.0);
        }
    }
    struct parser_node* last_parser_node = get_last_parser_node(parser_node);
    while (last_parser_node->is_root == 0) {
        push_polish(list, 1, last_parser_node->operation, 0.0);
        struct parser_node* temp = last_parser_node;
        last_parser_node = last_parser_node->prev;
        remove_last_parser_node(temp);
    }

    free(parser_node);
    return list;
}

int is_digit(char c) { return c >= '0' && c <= '9'; }

int is_dot(char c) { return c == '.'; }

int parse_double(char* string, double* value, int* shift) {
    int contains_dot = 0;
    int i = 0;
    int error = 0;
    int done = 0;
    int before_dot = 0;
    double after_dot = 0.0;
    double after_dot_scale = 0.1;
    while ((is_digit(*(string + i)) || (is_dot(*(string + i)) && !contains_dot)) && !done) {
        if (is_dot(*(string + i))) {
            if (contains_dot) {
                error = 1;
            } else {
                contains_dot = 1;
            }
        } else if (is_digit(*(string + i))) {
            int n = (int)(*(string + i) - 48);
            if (contains_dot) {
                after_dot = after_dot + n * after_dot_scale;
                after_dot_scale = after_dot_scale / 10.0;
            } else {
                before_dot = before_dot * 10 + n;
            }
        } else {
            done = 1;
        }
        i++;
    }
    *value = 1.0 * before_dot + after_dot;
    *shift = i;

    return error;
}