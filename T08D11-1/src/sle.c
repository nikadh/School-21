#include <math.h>
#include <stdio.h>
#include <stdlib.h>
void input(double **matrix, int *n, int *m);
void output(double *roots, int n);
void gauss(double **matrix, int n, double *roots);
void output_roots(double *roots, int n);
int main() {
    int n, m;
    if (scanf("%d %d", &n, &m) != 2 || n <= 0 || m != n + 1) {
        printf("n/a\n");
        return 1;
    }

    double **matrix = (double **)malloc(n * sizeof(double *));
    for (int i = 0; i < n; ++i) {
        matrix[i] = (double *)malloc((n + 1) * sizeof(double));
    }

    double *roots = (double *)malloc(n * sizeof(double));

    input(matrix, &n, &m);

    gauss(matrix, n, roots);

    output_roots(roots, n);

    for (int i = 0; i < n; ++i) {
        free(matrix[i]);
    }
    free(matrix);
    free(roots);

    return 0;
}
void input(double **matrix, int *n, int *m) {
    for (int i = 0; i < *n; ++i) {
        for (int j = 0; j < *m; ++j) {
            if (scanf("%lf", &matrix[i][j]) != 1) {
                printf("n/a\n");
                exit(1);
            }
        }
    }
}

void output(double *roots, int n) {
    for (int i = 0; i < n; ++i) {
        printf("%.6lf", roots[i]);
        if (i < n - 1) {
            printf(" ");
        }
    }
    printf("\n");
}

void gauss(double **matrix, int n, double *roots) {
    for (int i = 0; i < n; ++i) {
        int maxRow = i;
        for (int k = i + 1; k < n; ++k) {
            if (fabs(matrix[k][i]) > fabs(matrix[maxRow][i])) {
                maxRow = k;
            }
        }
        if (fabs(matrix[maxRow][i]) < 1e-12) {
            printf("n/a\n");
            exit(1);
        }

        if (i != maxRow) {
            double *temp = matrix[i];
            matrix[i] = matrix[maxRow];
            matrix[maxRow] = temp;
        }

        for (int k = i + 1; k < n; ++k) {
            double factor = matrix[k][i] / matrix[i][i];
            for (int j = i; j <= n; ++j) {
                matrix[k][j] -= factor * matrix[i][j];
            }
        }
    }

    for (int i = n - 1; i >= 0; --i) {
        roots[i] = matrix[i][n];
        for (int j = i + 1; j < n; ++j) {
            roots[i] -= matrix[i][j] * roots[j];
        }
        roots[i] /= matrix[i][i];
    }
}

void output_roots(double *roots, int n) { output(roots, n); }
