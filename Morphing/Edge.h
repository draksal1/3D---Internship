#pragma once

#include <QOpenGLShaderProgram>
#include <QScreen>
#include <QtMath>
#include <vector>

class EdgeGenerator {
public:
    EdgeGenerator(const int N) : N(N) {
        verticles = std::vector<GLfloat>(N * N * 3);
        indices = std::vector<GLuint>((N - 1) * (N - 1) * 3 * 2);
        colors = std::vector<GLfloat>(N * N * 3);
        generateNewPartition(N);
    }

    void generateNewPartition(int NewN);

    GLfloat *getVerticlesData() { return verticles.data(); };
    int getVerticlesSize() { return verticles.size(); };

    GLuint *getIndicesData() { return indices.data(); };
    int getIndicesSize() { return indices.size(); };

    GLfloat *getColorsData() { return colors.data(); };
    int getColorsSize() { return colors.size(); };

    int getN() { return N; };

private:
    std::vector<GLfloat> verticles;
    std::vector<GLuint> indices;
    std::vector<GLfloat> colors;
    int N;
    void generateVertices(GLfloat *SquareVertexes, int N);
    void generaterIndexes(int NewN);
};
