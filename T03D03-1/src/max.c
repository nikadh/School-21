#include <stdio.h>
int max(int a, int b);

int main() {
    int a, b;
    char c;
    if ((scanf("%d %d%c", &a, &b, &c) != 3) || c != '\n') {
        printf("n/a\n");
    } else {
        fflush(stdin);
        if ((a % 1 != 0) || (b % 1 != 0)) {
            printf("n/a\n");
        } else {
            printf("%d\n", max(a, b));
        }
    }
    return 0;
}
int max(int a, int b) { return (a > b) ? a : b; }
