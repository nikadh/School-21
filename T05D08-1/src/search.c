#include <math.h>
#include <stdio.h>

#define NMAX 30

// Функция для ввода массива чисел
int input(int *a, int *n);

// Функция для вывода массива чисел
void output(int *a, int n);

// Функция для вычисления среднего значения
double mean(int *a, int n);

// Функция для вычисления стандартного отклонения
double stddev(int *a, int n);

// Функция для поиска первого числа, удовлетворяющего условиям
int find_number(int *a, int n);

int main() {
    int n, data[NMAX];
    if (input(data, &n)) {
        int result = find_number(data, n);
        printf("%d\n", result);
    } else {
        printf("n/a\n");
    }
    return 0;
}

// Функция для ввода массива чисел
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

// Функция для вывода массива чисел
void output(int *a, int n) {
    for (int i = 0; i < n; i++) {
        printf("%d ", a[i]);
    }
    printf("\n");
}

// Функция для вычисления среднего значения
double mean(int *a, int n) {
    double sum = 0;
    for (int i = 0; i < n; i++) {
        sum += a[i];
    }
    return sum / n;
}

// Функция для вычисления стандартного отклонения
double stddev(int *a, int n) {
    double m = mean(a, n);
    double var = 0;
    for (int i = 0; i < n; i++) {
        var += (a[i] - m) * (a[i] - m);
    }
    return sqrt(var / n);
}

// Функция для поиска первого числа, удовлетворяющего условиям
int find_number(int *a, int n) {
    double mean_v = mean(a, n);
    double stddev_v = stddev(a, n);
    for (int i = 0; i < n; i++) {
        if (a[i] % 2 == 0 && a[i] >= mean_v && fabs(a[i] - mean_v) <= 3 * stddev_v && a[i] != 0) {
            return a[i];
        }
    }
    return 0;
}