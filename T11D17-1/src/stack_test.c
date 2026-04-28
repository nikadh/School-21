#include "stack.h"

#include <stdio.h>
void test();
int main() { test(); }

void test() {
    struct elem *head = init(33);
    printf("INI 33\n");
    printf("%d\n", head->val);
    if (head->val == 33)
        printf("SUCCESS\n");
    else
        printf("FAIL\n");
    push(&head, -333);
    printf("PUSH -333\n");
    printf("%d\n", head->val);
    if (head->val == -333)
        printf("SUCCESS\n");
    else
        printf("FAIL\n");

    printf("POP -333\n");

    int res = pop(&head);
    printf("%d\n", res);
    if (res == -333)
        printf("SUCCESS\n");
    else
        printf("FAIL\n");

    printf("NOW 33\n");
    printf("%d\n", head->val);
    if (head->val == 33)
        printf("SUCCESS\n");
    else
        printf("FAIL\n");
    destroy(&head);
}