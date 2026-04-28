#ifndef LIST_H
#define LIST_H

#include <stdlib.h>

#include "door_struct.h"
#define SUCCESS 0
#define FAIL -1

struct node {
    struct door *door;
    struct node *next;
};

struct node *init(struct door *door);
struct node *add_door(struct node *elem, struct door *door);
struct node *find_door(int door_id, struct node *root);
struct node *remove_door(struct node *elem, struct node *root);
void destroy(struct node *root);

#endif