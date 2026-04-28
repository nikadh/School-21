#include "prepare.h"

#include <stdio.h>
#include <string.h>

#define CHAR_DIFF ('a' - 'A')

int is_low(char str);
void set_substr(char *dest, char *substring);

void prepare(char *target, char *source) {
    to_uppercase(source);
    replace(target, source, "(-", "(0-");
    strcpy(source, target);
    replace(target, source, SIN, SHORT_SIN);
    strcpy(source, target);
    replace(target, source, COS, SHORT_COS);
    strcpy(source, target);
    replace(target, source, TAN, SHORT_TAN);
    strcpy(source, target);
    replace(target, source, CTG, SHORT_CTG);
    strcpy(source, target);
    replace(target, source, SQRT, SHORT_SQRT);
    strcpy(source, target);
    replace(target, source, LN, SHORT_LN);
}

void to_uppercase(char *str) {
    for (; *str != '\0'; str++) {
        if (is_low(*str)) {
            *str -= CHAR_DIFF;
        }
    }
}

int is_low(char c) { return c >= 'a' && c <= 'z'; }

void replace(char *target, char *source, char *find, char *replacement) {
    int find_len = strlen(find);
    int replacement_len = strlen(replacement);

    for (; *source != '\0'; source++, target++) {
        if (*source == *find) {
            char subbuff[find_len + 1];
            memcpy(subbuff, source, find_len);
            subbuff[find_len] = '\0';
            if (strcmp(subbuff, find) == 0) {
                set_substr(target, replacement);
                source += find_len - 1;
                target += replacement_len - 1;
            } else {
                *target = *source;
            }
        } else {
            *target = *source;
        }
    }
    *target = '\0';
}

void set_substr(char *target, char *substring) {
    for (; *substring != '\0'; target++, substring++) {
        *target = *substring;
    }
}
