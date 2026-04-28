#ifndef PREPARE_H
#define PREPARE_H

#define SIN "SIN"
#define COS "COS"
#define TAN "TAN"
#define CTG "CTG"
#define SQRT "SQRT"
#define LN "LN"

#define SHORT_SIN "S"
#define SHORT_COS "C"
#define SHORT_TAN "T"
#define SHORT_CTG "G"
#define SHORT_SQRT "Q"
#define SHORT_LN "L"

void prepare(char *target, char *source);
void to_uppercase(char *str);
void replace(char *target, char *source, char *find, char *replacment);

#endif
