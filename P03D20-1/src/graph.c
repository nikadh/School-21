#include <stdio.h>
#include <stdlib.h>

#include "helpers/io.h"
#include "helpers/prepare.h"
#include "polish/parser.h"

int main() {
    char source[4096] = "";
    input(source);
    char target[4096] = "";
    prepare(target, source);
    struct polish *polish = parse(target);

    int graph[25][80] = {0};
    init_matrix(graph);

    fill_matrix(polish, graph);

    draw_matrix(graph);
    printf("\n");
    destroy_polish(polish);

    return 0;
}
