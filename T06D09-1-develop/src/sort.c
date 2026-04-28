#include <stdio.h>
#define NMAX 10
int input(int *data);
void output(int *data);
void sorting(int *data);

int main() {
    int data[NMAX];
    if (input(data)) {
        sorting(data);
        output(data);
    } else {
        printf("n/a");
    }
    return 0;
}

int input(int *data) {
    int flag = 1;
    for (int i = 0; i < NMAX; i++) {
        if (scanf("%d", &data[i]) != 1 && flag == 1) {
            flag = 0;
        }
    }
    return flag;
}
void output(int *data) {
    for (int i = 0; i < NMAX; i++) {
        printf("%d", data[i]);
        if (i < NMAX - 1) {
            printf(" ");
        }
    }
}
void sorting(int *data) {
    int q = 0;
    for (int i = 0; i < NMAX; i++) {
        for (int j = 0; j < NMAX - i - 1; j++) {
            if (data[j] > data[j + 1]) {
                q = data[j];
                data[j] = data[j + 1];
                data[j + 1] = q;
            }
        }
    }
}