#include "list.h"

#include <stdio.h>

int test_add_door() {
    struct door *door1 = (struct door *)malloc(sizeof(struct door));
    door1->id = 1;
    struct node *list = init(door1);

    struct door *door2 = (struct door *)malloc(sizeof(struct door));
    door2->id = 2;
    add_door(list, door2);

    if (list->next->door->id == 2) {
        destroy(list);
        return SUCCESS;
    }

    destroy(list);
    return FAIL;
}

int test_remove_door() {
    struct door *door1 = (struct door *)malloc(sizeof(struct door));
    door1->id = 1;
    struct node *list = init(door1);

    struct door *door2 = (struct door *)malloc(sizeof(struct door));
    door2->id = 2;
    add_door(list, door2);

    list = remove_door(list->next, list);

    if (list->next == NULL) {
        destroy(list);
        return SUCCESS;
    }

    destroy(list);
    return FAIL;
}

int test_find_door() {
    struct door *door1 = (struct door *)malloc(sizeof(struct door));
    door1->id = 1;
    struct node *list = init(door1);

    struct door *door2 = (struct door *)malloc(sizeof(struct door));
    door2->id = 2;
    add_door(list, door2);

    struct door *door3 = (struct door *)malloc(sizeof(struct door));
    door3->id = 3;
    add_door(list, door3);

    // Проверяем, что функция find_door находит дверь с id = 2
    struct node *found_node = find_door(2, list);
    if (found_node && found_node->door->id == 2) {
        destroy(list);
        return SUCCESS;
    }

    destroy(list);
    return FAIL;
}

int main() {
    int result = SUCCESS;
    result &= test_add_door();
    result &= test_remove_door();
    result &= test_find_door();  // Добавляем вызов теста find_door

    if (result == SUCCESS) {
        printf("All tests passed!\n");
    } else {
        printf("Some tests failed.\n");
    }

    return result;
}