#include "trianglewindow.h"
#include <QApplication>
#include <QDir>

#define aspectRatio 16.0f / 9.0f
#define verticalAngle 70.0f
#define nearPlane 0.1f
#define farPlane 100.0f
#define distancing -3.0f
#define cameraAngle 35.0f

void TriangleWindow::initialize() {

    m_program = new QOpenGLShaderProgram(this);
    m_program->addShaderFromSourceFile(QOpenGLShader::Vertex,
                                       ".//..//shader.vsh");
    m_program->addShaderFromSourceFile(QOpenGLShader::Fragment,
                                       ".//..//fragmentShader.fsh");
    m_program->link();
    m_posAttr = m_program->attributeLocation("posAttr");
    Q_ASSERT(m_posAttr != -1);
    m_colAttr = m_program->attributeLocation("colAttr");
    Q_ASSERT(m_colAttr != -1);
    m_matrixUniform = m_program->uniformLocation("matrix");
    Q_ASSERT(m_matrixUniform != -1);
    m_morphParam = m_program->uniformLocation("morph");
    Q_ASSERT(m_morphParam != -1);
}

void TriangleWindow::render() {
    const qreal retinaScale = devicePixelRatio();
    glViewport(0, 0, width() * retinaScale, height() * retinaScale);

    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

    m_program->bind();
    m_program->setUniformValue(m_morphParam, m_uniform);
    QMatrix4x4 matrix;
    matrix.perspective(verticalAngle, aspectRatio, nearPlane, farPlane);
    matrix.translate(0, 0.0, distancing);
    matrix.rotate(cameraAngle, 1, 1, 1);
    m_program->setUniformValue(m_matrixUniform, matrix);

    GLuint EBO;
    glGenBuffers(1, &EBO);
    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);

    glBufferData(GL_ELEMENT_ARRAY_BUFFER, edge.getIndicesSize() * sizeof(GLuint),
                 edge.getIndicesData(), GL_STATIC_DRAW);

    glVertexAttribPointer(m_posAttr, 3, GL_FLOAT, GL_FALSE, 0,
                          edge.getVerticlesData());
    glVertexAttribPointer(m_colAttr, 3, GL_FLOAT, GL_FALSE, 0,
                          edge.getColorsData());

    glEnableVertexAttribArray(m_posAttr);
    glEnableVertexAttribArray(m_colAttr);

    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
    glDrawElements(GL_TRIANGLES, (edge.getN() - 1) * (edge.getN() - 1) * 3 * 2,
                   GL_UNSIGNED_INT, 0);

    for (int i = 0; i < 3; i++) {
        matrix.rotate(90.0f, 1, 0, 0);
        m_program->setUniformValue(m_matrixUniform, matrix);
        glDrawElements(GL_TRIANGLES, (edge.getN() - 1) * (edge.getN() - 1) * 3 * 2,
                       GL_UNSIGNED_INT, 0);
    }
    matrix.rotate(90.0f, 0, 0, 1);
    m_program->setUniformValue(m_matrixUniform, matrix);
    glDrawElements(GL_TRIANGLES, (edge.getN() - 1) * (edge.getN() - 1) * 3 * 2,
                   GL_UNSIGNED_INT, 0);

    matrix.rotate(180.0f, 0, 0, 1);
    m_program->setUniformValue(m_matrixUniform, matrix);
    glDrawElements(GL_TRIANGLES, (edge.getN() - 1) * (edge.getN() - 1) * 3 * 2,
                   GL_UNSIGNED_INT, 0);

    glCullFace(GL_FRONT);
    glEnable(GL_CULL_FACE);

    glDisableVertexAttribArray(m_colAttr);
    glDisableVertexAttribArray(m_posAttr);

    m_program->setUniformValue(m_matrixUniform, matrix);
    m_program->release();

    ++m_frame;
}
