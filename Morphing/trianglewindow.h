#pragma once

#include "Edge.h"
#include "openglwindow.h"
#include <QMatrix4x4>
#include <QScreen>
#include <QtMath>

class TriangleWindow : public OpenGLWindow {
public:
    using OpenGLWindow::OpenGLWindow;

    void initialize() override;
    void render() override;

    void setUniform(int uniform) {
        m_uniform = static_cast<GLfloat>(uniform) / 100;
    };

    void setN(int N) { edge.generateNewPartition(N); };

private:
    GLfloat m_uniform;
    GLint m_posAttr = 0;
    GLint m_colAttr = 0;
    GLint m_matrixUniform = 0;
    GLint m_morphParam = 0;
    EdgeGenerator edge = EdgeGenerator(2);
    QOpenGLShaderProgram *m_program = nullptr;
    int m_frame = 0;
};
