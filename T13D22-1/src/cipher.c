#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define MAX_PATH_LENGTH 256
#define MAX_BUFFER_SIZE 512

void showMenu(FILE *currentFile, char *currentFileName, int userChoice);
void openFile(FILE **fileHandle, char *fileName);
void displayFileContents(FILE *fileHandle);
int isFileEmpty(FILE *fileHandle);
FILE *appendToFile(const char *fileName, const char *data);
void processOptionTwo(FILE *currentFile, char *currentFileName);
void processOptionOne(FILE *currentFile, char *currentFileName);
void clearHeaderFiles(char *homeDirectory);
void performCaesarCipher(char *text, int shiftAmount);
void processOptionThree();
FILE *createFileWithContent(const char *fileName, const char *data);
int doesFileExist(FILE *fileHandle, char *fileName);

int main() {
    FILE *currentFile = NULL;
    int userChoice = 0;
    char currentFileName[MAX_PATH_LENGTH];
    while (userChoice != -1) {
        if (scanf("%d", &userChoice) != 1) {
            printf("n/a\n");
            while (getchar() != '\n')
                ;
            continue;
        }
        if (userChoice != -1) {
            showMenu(currentFile, currentFileName, userChoice);
        }
    }
    return 0;
}

void showMenu(FILE *currentFile, char *currentFileName, int userChoice) {
    switch (userChoice) {
        case 1: {
            processOptionOne(currentFile, currentFileName);
            break;
        }
        case 2: {
            processOptionTwo(currentFile, currentFileName);
            break;
        }
        case 3:
            processOptionThree();
            break;
        default:
            printf("n/a\n");
            break;
    }
}

void processOptionThree() {
    int shiftAmount;
    char filePath1[256];
    char filePath2[256];
    char buffer1[512] = {0};
    char buffer2[512] = {0};
    char *homeDirectory = getenv("HOME");
    snprintf(filePath1, sizeof(filePath1), "%s/T13D22-1/src/ai_modules/m1.c", homeDirectory);
    snprintf(filePath2, sizeof(filePath2), "%s/T13D22-1/src/ai_modules/m2.c", homeDirectory);
    FILE *file1;
    FILE *file2;
    openFile(&file1, filePath1);
    openFile(&file2, filePath2);
    int index = 0;
    char charFromFile1;
    char charFromFile2;
    if (file1) {
        while ((charFromFile1 = fgetc(file1)) != EOF) {
            buffer1[index++] = charFromFile1;
        }
        buffer1[index] = 0;
        fclose(file1);
    } else {
        printf("Не удалось открыть файл %s\n", filePath1);
    }
    index = 0;
    if (file2) {
        while ((charFromFile2 = fgetc(file2)) != EOF) {
            buffer2[index++] = charFromFile2;
        }
        buffer2[index] = 0;
        fclose(file2);
    } else {
        printf("Не удалось открыть файл %s\n", filePath2);
    }

    clearHeaderFiles(homeDirectory);

    if (scanf("%d", &shiftAmount) != 1) {
        printf("n/a\n");
    } else {
        performCaesarCipher(buffer1, shiftAmount);
        performCaesarCipher(buffer2, shiftAmount);
        createFileWithContent(filePath1, buffer1);
        createFileWithContent(filePath2, buffer2);
    }
}

void performCaesarCipher(char *text, int shiftAmount) {
    char character;
    for (int i = 0; text[i] != '\0'; ++i) {
        character = text[i];
        if (character >= 'a' && character <= 'z') {
            character = (character - 'a' + shiftAmount) % 26 + 'a';
            text[i] = character;
        } else if (character >= 'A' && character <= 'Z') {
            character = (character - 'A' + shiftAmount) % 26 + 'A';
            text[i] = character;
        }
    }
}

void clearHeaderFiles(char *homeDirectory) {
    char headerFilePath1[256];
    char headerFilePath2[256];

    snprintf(headerFilePath1, sizeof(headerFilePath1), "%s/T13D22-1/src/ai_modules/m1.h", homeDirectory);
    snprintf(headerFilePath2, sizeof(headerFilePath2), "%s/T13D22-1/src/ai_modules/m2.h", homeDirectory);

    FILE *fileHandle = fopen(headerFilePath1, "w");
    if (fileHandle) {
        fclose(fileHandle);
    }

    fileHandle = fopen(headerFilePath2, "w");
    if (fileHandle) {
        fclose(fileHandle);
    }
}

FILE *createFileWithContent(const char *fileName, const char *data) {
    FILE *fileHandle = fopen(fileName, "w");
    if (fileHandle != NULL) {
        fputs(data, fileHandle);
        fclose(fileHandle);
    }
    return fileHandle;
}

void processOptionOne(FILE *currentFile, char *currentFileName) {
    if (scanf("%s", currentFileName) != 1) {
        printf("n/a\n");
    } else {
        openFile(&currentFile, currentFileName);
        if (currentFile != NULL && !isFileEmpty(currentFile)) {
            displayFileContents(currentFile);
            fclose(currentFile);
        } else {
            printf("n/a\n");
        }
    }
}

void processOptionTwo(FILE *currentFile, char *currentFileName) {
    char inputBuffer[MAX_BUFFER_SIZE];
    while (getchar() != '\n')
        ;
    fgets(inputBuffer, sizeof(inputBuffer), stdin);
    if ((doesFileExist(currentFile, currentFileName)) && (strlen(currentFileName) > 0)) {
        inputBuffer[strcspn(inputBuffer, "\n")] = '\0';
        appendToFile(currentFileName, inputBuffer);
        fclose(currentFile);
        openFile(&currentFile, currentFileName);
        displayFileContents(currentFile);
        fclose(currentFile);
    } else {
        printf("n/a\n");
    }
}

void openFile(FILE **fileHandle, char *fileName) { *fileHandle = fopen(fileName, "r"); }

int doesFileExist(FILE *fileHandle, char *fileName) {
    int exists = 0;
    fileHandle = fopen(fileName, "r");
    if (fileHandle != NULL) {
        exists = 1;
        fclose(fileHandle);
    }
    return exists;
}

FILE *appendToFile(const char *fileName, const char *data) {
    FILE *fileHandle = fopen(fileName, "a");
    if (fileHandle != NULL) {
        fputs(data, fileHandle);
        fclose(fileHandle);
    }
    return fileHandle;
}

int isFileEmpty(FILE *fileHandle) {
    fseek(fileHandle, 0, SEEK_END);
    long fileSize = ftell(fileHandle);
    fseek(fileHandle, 0, SEEK_SET);
    return fileSize == 0;
}

void displayFileContents(FILE *fileHandle) {
    char lineBuffer[MAX_BUFFER_SIZE];
    while (fgets(lineBuffer, sizeof(lineBuffer), fileHandle) != NULL) {
        printf("%s", lineBuffer);
    }
    printf("\n");
}
