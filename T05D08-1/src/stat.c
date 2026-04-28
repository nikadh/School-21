#include <stdio.h>
#define NMAX 10

int input(int *a, int *n);
void output(int *a, int n);
void squaring(int *a, int n);
void output_result(int max_v, int min_v, double mean_v, double variance_v);
int max(int *a, int n);
int min(int *a, int n);
double mean(int *a, int n);
double variance(int *a, int n);

int main() {
    int n, data[NMAX];
    if (input(data, &n)) {
        double mean_v = mean(data, n);
        double variance_v = variance(data, n);
        squaring(data, n);
        output(data, n);
        int max_v = max(data, n);
        int min_v = min(data, n);
        output_result(max_v, min_v, mean_v, variance_v);
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
void output_result(int max_v, int min_v, double mean_v, double variance_v) {
    printf("%d %d %.6f %.6f", min_v, max_v, mean_v, variance_v);
}
void output(int *a, int n) {
    for (int i = 0; i < n; i++) {
        printf("%d ", a[i]);
    }
    printf("\n");
    return;
}

double variance(int *a, int n) {
    double m = mean(a, n);
    double var = 0;
    for (int i = 0; i < n; i++) {
        var += (a[i] - m) * (a[i] - m);
    }
    return var / n;
}
void squaring(int *a, int n) {
    for (int i = 0; i < n; i++) {
        a[i] *= a[i];
    }
    return;
}
double mean(int *a, int n) {
    double sum = 0;
    for (int i = 0; i < n; i++) {
        sum += a[i];
    }
    return sum / n;
}
int max(int *a, int n) {
    int max_v = a[0];
    for (int i = 1; i < n; i++) {
        if (a[i] > max_v) {
            max_v = a[i];
        }
    }
    return max_v;
}

int min(int *a, int n) {
    int min_v = a[0];
    for (int i = 1; i < n; i++) {
        if (a[i] < min_v) {
            min_v = a[i];
        }
    }
    return min_v;
}