#include <stdio.h>
int main() {
    int a, b, sum, minus, pro, chas;
    char c;
    if ((scanf("%d %d%c", &a, &b, &c)) != 3 || c != '\n') {
        printf("n/a\n");
    } else {
        if ((a % 1 != 0) || (b % 1 != 0)) {
            printf("n/a\n");
        } else {
            sum = a + b;
            printf("%d ", sum);
            minus = a - b;
            printf("%d ", minus);
            pro = a * b;
            printf("%d ", pro);
            if (b == 0) {
                printf("n/a\n");
            } else {
                chas = a / b;
                printf("%d\n", chas);
            }
        }
    }

    return 0;
}