#include <stdio.h>
#define NMAX 10

int input(int *a, int *n);
void output(int *a, int n);
void squaring(int *a, int n);

int main() {
    int n, data[NMAX];
    if (input(data, &n)) {
        squaring(data, n);
        output(data, n);
    } else {
        printf("n/a");
    }

    return 0;
}

int input(int *a, int *n) {
    char c;
    int check = 1;
    if ((scanf("%d%c", n, &c) != 2) || (*n > NMAX) || (*n <= 0) || (c != '\n')) {
        check = 0;
    } else {
        for (int i = 0; i < *n; i++) {
            if ((scanf("%d%c", &a[i], &c) != 2) || (i < *n - 1 && c != ' ') ||
                ((i == *n - 1) && (c == ' '))) {
                check = 0;
                break;
            }
        }
        if (check && (c != '\n' && c != ' ')) {
            check = 0;
        }
    }
    return check;
}

void output(int *a, int n) {
    for (int i = 0; i < n; i++) {
        printf("%d ", a[i]);
    }
    return;
}

void squaring(int *a, int n) {
    for (int i = 0; i < n; i++) {
        a[i] *= a[i];
    }
    return;
}
