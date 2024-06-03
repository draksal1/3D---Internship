#include "Edge.h"

void EdgeGenerator::generateVertices(GLfloat *SquareVertexes, int N) {
    GLfloat Xstart = SquareVertexes[0];
    GLfloat Ystart = SquareVertexes[1];
    GLfloat Zstart = SquareVertexes[2];

    GLfloat DX1 = (SquareVertexes[3] - Xstart) / (N - 1);
    GLfloat DY1 = (SquareVertexes[4] - Ystart) / (N - 1);
    GLfloat DZ1 = (SquareVertexes[5] - Zstart) / (N - 1);

    GLfloat DX2 = (SquareVertexes[9] - Xstart) / (N - 1);
    GLfloat DY2 = (SquareVertexes[10] - Ystart) / (N - 1);
    GLfloat DZ2 = (SquareVertexes[11] - Zstart) / (N - 1);

    for (int i = 0; i < N; i++) {
        for (int j = 0; j < N; j++) {
            verticles[(i * N + j) * 3] = Xstart + DX1 * i + DX2 * j;
            verticles[(i * N + j) * 3 + 1] = Ystart + DY1 * i + DY2 * j;
            verticles[(i * N + j) * 3 + 2] = Zstart + DZ1 * i + DZ2 * j;
        }
    }
}

void EdgeGenerator::generaterIndexes(int N) {
    for (int i = 0; i <= N - 2; i++) {
        for (int j = 0; j <= N - 2; j++) {
            indices[((N - 1) * i + j) * 3] = i * N + j;
            indices[((N - 1) * i + j) * 3 + 2] = i * N + j + 1;
            indices[((N - 1) * i + j) * 3 + 1] = (i + 1) * N + j;
        }
    }
    for (int i = 0; i <= N - 2; i++) {
        for (int j = 0; j <= N - 2; j++) {
            indices[((N - 1) * i + j) * 3 + (N - 1) * (N - 1) * 3] = (i + 1) * N + j;
            indices[((N - 1) * i + j) * 3 + 2 + (N - 1) * (N - 1) * 3] =
                i * N + j + 1;
            indices[((N - 1) * i + j) * 3 + 1 + (N - 1) * (N - 1) * 3] =
                (i + 1) * N + j + 1;
        }
    }
}

void EdgeGenerator::generateNewPartition(int NewN) {
    if (NewN != N) {
        N = NewN;
        verticles.resize(N * N * 3);
        indices.resize((N - 1) * (N - 1) * 3 * 2);
        colors.resize(N * N * 3);
    }
    GLfloat Edge[] = {
        1.0f,  1.0f, 1.0f,  -1.0f, 1.0f, 1.0f,
        -1.0f, 1.0f, -1.0f, 1.0f,  1.0f, -1.0f,
    };
    generateVertices(Edge, N);
    generaterIndexes(N);

    for (int i = 0; i < N; i++) {
        for (int j = 0; j < N; j++) {
            if (i <= N / 3) {
                colors[(i * N + j) * 3] = 1.0f;
                colors[(i * N + j) * 3 + 1] = 0.0f;
                colors[(i * N + j) * 3 + 2] = 0.0f;
            }
            if (i > N / 3 && i <= 2 * N / 3) {
                colors[(i * N + j) * 3] = 0.0f;
                colors[(i * N + j) * 3 + 1] = 0.0f;
                colors[(i * N + j) * 3 + 2] = 1.0f;
            }
            if (i > 2 * N / 3) {
                colors[(i * N + j) * 3] = 0.0f;
                colors[(i * N + j) * 3 + 1] = 1.0f;
                colors[(i * N + j) * 3 + 2] = 0.0f;
            }
        }
    }
}
