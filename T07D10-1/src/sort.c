#include <stdio.h>
#include <stdlib.h>

int input(int **data, int *n);
void output(int *data, int n);
void sorting(int *data, int n);

int main() {
    int *data = NULL;
    int n;
    if (input(&data, &n)) {
        sorting(data, n);
        output(data, n);
        free(data);
    } else {
        printf("n/a");
    }
    return 0;
}

int input(int **data, int *n) {
    int flag = 1;
    if (scanf("%d", n) != 1) {
        flag = 0;
    } else {
        *data = (int *)calloc(*n, sizeof(int));
        if (*data == NULL) {
            flag = 0;
        } else {
            for (int i = 0; i < *n; i++) {
                if (scanf("%d", &(*data)[i]) != 1 && flag == 1) {
                    flag = 0;
                }
            }
        }
    }
    return flag;
}

void output(int *data, int n) {
    for (int i = 0; i < n; i++) {
        printf("%d", data[i]);
        if (i < n - 1) {
            printf(" ");
        }
    }
}

void sorting(int *data, int n) {
    int q = 0;
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n - i - 1; j++) {
            if (data[j] > data[j + 1]) {
                q = data[j];
                data[j] = data[j + 1];
                data[j + 1] = q;
            }
        }
    }
}