#include <stdio.h>
#include <stdlib.h>

void sort_2(int **matrix, int n, int m);

int inputsize(int ***a, int *n, int *m);
void dinamic(int ***a, int n, int m);
int input(int ***a, int n, int m);
void output(int **a, int n, int m);
int chercorrect(char symbol);
int sizearraycorrect(int s);
void copy(int **a, int n, int m, int **cpy);
void sort_horizontal(int **a, int n, int m, int *src);
void sort_vertical(int **a, int n, int m, int *src);

int main() {
    int **matrix;
    int n, m;
    int *res;

    if (inputsize(&matrix, &n, &m)) {
        sort_2(matrix, n, m);
        copy(matrix, n, m, &res);
        sort_vertical(matrix, n, m, res);
        output(matrix, n, m);
        printf("\n\n");
        sort_horizontal(matrix, n, m, res);
        output(matrix, n, m);
        free(res);
        for (int i = 0; i < n; i++) free(matrix[i]);
        free(matrix);
    } else {
        if (n != 0 && m != 0) {
            for (int i = 0; i < n; i++) free(matrix[i]);
            free(matrix);
        }
        printf("n/a");
    }
}
void output(int **a, int n, int m) {
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < m; j++) {
            if (j != m - 1)
                printf("%d ", a[i][j]);
            else
                printf("%d", a[i][j]);
        }
        if (i != n - 1) printf("\n");
    }
}
int chercorrect(char symbol) { return symbol == ' ' || symbol == '\n' ? 1 : 0; }
int sizearraycorrect(int s) { return s > 0 ? 1 : 0; }

int inputsize(int ***a, int *n, int *m) {
    char symbol;
    int check = 1;

    if (scanf("%d %d%c", n, m, &symbol) == 3 && sizearraycorrect(*n) && sizearraycorrect(*m) &&
        chercorrect(symbol)) {
        dinamic(a, *n, *m);
        check = input(a, *n, *m);
    } else {
        check = 0;
    }
    return check;
}

void dinamic(int ***a, int n, int m) {
    (*a) = malloc(n * sizeof(int *));
    for (int i = 0; i < n; i++) {
        (*a)[i] = malloc(m * sizeof(int));
    }
}

int input(int ***a, int n, int m) {
    char symbol;
    int el;
    int check = 1;
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < m; j++) {
            if (scanf("%d%c", &el, &symbol) == 2 && chercorrect(symbol)) {
                (*a)[i][j] = el;
            } else {
                check = 0;
                break;
            }
        }
    }
    return check;
}

void sort_2(int **matrix, int n, int m) {
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < m; j++) {
            for (int k = 0; k < n; k++) {
                for (int q = 0; q < m; q++) {
                    if (matrix[i][j] < matrix[k][q]) {
                        int temp = matrix[i][j];
                        matrix[i][j] = matrix[k][q];
                        matrix[k][q] = temp;
                    }
                }
            }
        }
    }
}

void copy(int **a, int n, int m, int **cpy) {
    (*cpy) = malloc(n * m * sizeof(int));
    int k = 0;
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < m; j++) {
            (*cpy)[k] = a[i][j];
            k++;
        }
    }
}

void sort_horizontal(int **a, int n, int m, int *src) {
    for (int i = 0; i < n; i++) {
        if (i % 2 == 0) {
            for (int j = 0; j < m; j++) {
                a[i][j] = src[i * m + j];
            }
        } else {
            for (int j = m - 1; j >= 0; j--) {
                a[i][m - j - 1] = src[i * m + j];
            }
        }
    }
}
void sort_vertical(int **a, int n, int m, int *src) {
    for (int i = 0; i < m; i++) {
        if (i % 2 == 0) {
            for (int j = 0; j < n; j++) {
                a[j][i] = src[i * n + j];
            }
        } else {
            for (int j = n - 1; j >= 0; j--) {
                a[n - j - 1][i] = src[i * n + j];
            }
        }
    }
}
