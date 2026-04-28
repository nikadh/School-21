#include "data_stat.h"

#include <limits.h>
#include <stdlib.h>
double max(double *a, int n) {
    double maxi = INT_MIN;
    for (int i = 0; i < n; i++) {
        if (a[i] > maxi) maxi = a[i];
    }
    return maxi;
}
double min(double *a, int n) {
    double mini = INT_MAX;
    for (int i = 0; i < n; i++) {
        if (a[i] < mini) mini = a[i];
    }
    return mini;
}
double mean(double *data, int n) {
    double sum = 0;
    for (int i = 0; i < n; i++) {
        sum += *(data + i);
    }
    sum /= n;
    return sum;
}
void squareArray(double *data, int n) {
    for (int i = 0; i < n; i++) {
        *(data + i) = *(data + i) * *(data + i);
    }
}
double variance(double *data, int n) {
    double *squaredData = (double *)malloc(n * sizeof(double));
    for (int i = 0; i < n; i++) {
        *(squaredData + i) = *(data + i);
    }
    squareArray(squaredData, n);
    double squaredDataMean = mean(squaredData, n);
    free(squaredData);
    double dataMean = mean(data, n);
    double result = squaredDataMean - dataMean * dataMean;
    return result;
}