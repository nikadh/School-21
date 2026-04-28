#ifndef PARSE_H
#define PARSE_H

#include "polish.h"

struct parser_node {
    struct parser_node* next;
    struct parser_node* prev;
    int is_root;
    char operation;
};

struct parser_node* init_parser_node(int is_root, char operation);
struct parser_node* push_parser_node(struct parser_node* root, char operation);
struct parser_node* get_last_parser_node(struct parser_node* root);
void remove_last_parser_node(struct parser_node* root);
void destroy(struct parser_node* root);

struct polish* parse(char* source);
int is_digit(char c);
int is_dot(char c);
int parse_double(char* string, double* value, int* shift);

#endif
