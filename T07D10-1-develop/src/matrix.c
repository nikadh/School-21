#include <stdio.h>
#include <stdlib.h>

int main() {
    int choice;
    if (scanf("%d", &choice) != 1) {
        printf("n/a");
        return 1;
    }

    int rows, cols;
    if (scanf("%d %d", &rows, &cols) != 2) {
        printf("n/a");
        return 1;
    }

    int **matrix = NULL;
    switch (choice) {
        case 1: {
            if (rows > 100 || cols > 100) {
                printf("n/a");
                return 1;
            }
            int static_matrix[100][100];
            matrix = (int **)malloc(rows * sizeof(int *));
            if (matrix == NULL) {
                printf("n/a");
                return 1;
            }
            for (int i = 0; i < rows; i++) {
                matrix[i] = static_matrix[i];
            }
            break;
        }
        case 2: {
            matrix = (int **)malloc(rows * sizeof(int *));
            if (matrix == NULL) {
                printf("n/a");
                return 1;
            }
            for (int i = 0; i < rows; i++) {
                matrix[i] = (int *)malloc(cols * sizeof(int));
                if (matrix[i] == NULL) {
                    printf("n/a");
                    for (int j = 0; j < i; j++) {
                        free(matrix[j]);
                    }
                    free(matrix);
                    return 1;
                }
            }
            break;
        }
        case 3: {
            matrix = (int **)malloc(rows * sizeof(int *));
            if (matrix == NULL) {
                printf("n/a");
                return 1;
            }
            int *data = (int *)malloc(rows * cols * sizeof(int));
            if (data == NULL) {
                printf("n/a");
                free(matrix);
                return 1;
            }
            for (int i = 0; i < rows; i++) {
                matrix[i] = data + i * cols;
            }
            break;
        }
        case 4: {
            matrix = (int **)malloc(rows * sizeof(int *));
            if (matrix == NULL) {
                printf("n/a");
                return 1;
            }
            matrix[0] = (int *)malloc(rows * cols * sizeof(int));
            if (matrix[0] == NULL) {
                printf("n/a");
                free(matrix);
                return 1;
            }
            for (int i = 1; i < rows; i++) {
                matrix[i] = matrix[0] + i * cols;
            }
            break;
        }
        default:
            printf("n/a");
            return 1;
    }

    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            if (scanf("%d", &matrix[i][j]) != 1) {
                printf("n/a");
                if (choice == 2) {
                    for (int k = 0; k < rows; k++) {
                        free(matrix[k]);
                    }
                } else if (choice == 3) {
                    free(matrix[0]);
                } else if (choice == 4) {
                    free(matrix[0]);
                }
                free(matrix);
                return 1;
            }
        }
    }

    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            printf("%d", matrix[i][j]);
            if (j < cols - 1) {
                printf(" ");
            }
        }
        if (i < rows - 1) {
            printf("\n");
        }
    }

    if (choice == 2) {
        for (int i = 0; i < rows; i++) {
            free(matrix[i]);
        }
    } else if (choice == 3) {
        free(matrix[0]);
    } else if (choice == 4) {
        free(matrix[0]);
    }
    free(matrix);

    return 0;
}