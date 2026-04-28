#include <stdio.h>
int main() {
    float x, y, radius = 5.0;
    char c;
    if ((scanf("%f %f%c", &x, &y, &c)) != 3 || c != '\n') {
        printf("n/a\n");
    } else {
        if (x * x + y * y < radius * radius) {
            printf("GOTHA\n");
        } else {
            printf("MISS\n");
        }
    }
}