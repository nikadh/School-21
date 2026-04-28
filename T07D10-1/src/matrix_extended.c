#include <stdio.h>
#include <stdlib.h>

int **allocate_matrix(int choice, int rows, int cols);
void free_matrix(int **matrix, int choice, int rows);
int read_matrix(int **matrix, int rows, int cols);
void print_matrix(int **matrix, int rows, int cols);
void find_max_in_rows(int **matrix, int rows, int cols, int *max_row);
void find_min_in_cols(int **matrix, int rows, int cols, int *min_col);

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

    int **matrix = allocate_matrix(choice, rows, cols);
    if (matrix == NULL) {
        printf("n/a");
        return 1;
    }

    if (read_matrix(matrix, rows, cols) != 0) {
        printf("n/a");
        free_matrix(matrix, choice, rows);
        return 1;
    }

    print_matrix(matrix, rows, cols);

    int *max_row = (int *)malloc(rows * sizeof(int));
    int *min_col = (int *)malloc(cols * sizeof(int));

    if (max_row == NULL || min_col == NULL) {
        printf("n/a");
        free_matrix(matrix, choice, rows);
        free(max_row);
        free(min_col);
        return 1;
    }

    find_max_in_rows(matrix, rows, cols, max_row);
    find_min_in_cols(matrix, rows, cols, min_col);

    // Print the maximum elements in each row
    printf("\n");
    for (int i = 0; i < rows; i++) {
        printf("%d", max_row[i]);
        if (i < rows - 1) {
            printf(" ");
        }
    }

    // Print the minimum elements in each column
    printf("\n");
    for (int i = 0; i < cols; i++) {
        printf("%d", min_col[i]);
        if (i < cols - 1) {
            printf(" ");
        }
    }

    free(max_row);
    free(min_col);
    free_matrix(matrix, choice, rows);

    return 0;
}

// Allocate memory for the matrix based on the choice
int **allocate_matrix(int choice, int rows, int cols) {
    int **matrix = NULL;

    switch (choice) {
        case 1: {
            if (rows > 100 || cols > 100) {
                return NULL;
            }
            static int static_matrix[100][100];
            matrix = (int **)malloc(rows * sizeof(int *));
            if (matrix == NULL) {
                return NULL;
            }
            for (int i = 0; i < rows; i++) {
                matrix[i] = static_matrix[i];
            }
            break;
        }
        case 2: {
            matrix = (int **)malloc(rows * sizeof(int *));
            if (matrix == NULL) {
                return NULL;
            }
            for (int i = 0; i < rows; i++) {
                matrix[i] = (int *)malloc(cols * sizeof(int));
                if (matrix[i] == NULL) {
                    for (int j = 0; j < i; j++) {
                        free(matrix[j]);
                    }
                    free(matrix);
                    return NULL;
                }
            }
            break;
        }
        case 3: {
            matrix = (int **)malloc(rows * sizeof(int *));
            if (matrix == NULL) {
                return NULL;
            }
            int *data = (int *)malloc(rows * cols * sizeof(int));
            if (data == NULL) {
                free(matrix);
                return NULL;
            }
            for (int i = 0; i < rows; i++) {
                matrix[i] = data + i * cols;
            }
            break;
        }
        case 4: {
            matrix = (int **)malloc(rows * sizeof(int *));
            if (matrix == NULL) {
                return NULL;
            }
            matrix[0] = (int *)malloc(rows * cols * sizeof(int));
            if (matrix[0] == NULL) {
                free(matrix);
                return NULL;
            }
            for (int i = 1; i < rows; i++) {
                matrix[i] = matrix[0] + i * cols;
            }
            break;
        }
        default:
            return NULL;
    }

    return matrix;
}

// Free the allocated memory based on the choice
void free_matrix(int **matrix, int choice, int rows) {
    if (matrix == NULL) {
        return;
    }

    if (choice == 2) {
        for (int i = 0; i < rows; i++) {
            free(matrix[i]);
        }
    } else if (choice == 3 || choice == 4) {
        free(matrix[0]);
    }

    free(matrix);
}

// Read the matrix data from input
int read_matrix(int **matrix, int rows, int cols) {
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            if (scanf("%d", &matrix[i][j]) != 1) {
                return 1;  // Input error
            }
        }
    }
    return 0;
}

// Print the matrix to the console
void print_matrix(int **matrix, int rows, int cols) {
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
}

// Find the maximum elements in each row
void find_max_in_rows(int **matrix, int rows, int cols, int *max_row) {
    for (int i = 0; i < rows; i++) {
        int max_val = matrix[i][0];
        for (int j = 1; j < cols; j++) {
            if (matrix[i][j] > max_val) {
                max_val = matrix[i][j];
            }
        }
        max_row[i] = max_val;
    }
}

// Find the minimum elements in each column
void find_min_in_cols(int **matrix, int rows, int cols, int *min_col) {
    for (int j = 0; j < cols; j++) {
        int min_val = matrix[0][j];
        for (int i = 1; i < rows; i++) {
            if (matrix[i][j] < min_val) {
                min_val = matrix[i][j];
            }
        }
        min_col[j] = min_val;
    }
}