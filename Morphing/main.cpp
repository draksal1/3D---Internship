/****************************************************************************
**
** Copyright (C) 2016 The Qt Company Ltd.
** Contact: https://www.qt.io/licensing/
**
** This file is part of the documentation of the Qt Toolkit.
**
** $QT_BEGIN_LICENSE:BSD$
** Commercial License Usage
** Licensees holding valid commercial Qt licenses may use this file in
** accordance with the commercial license agreement provided with the
** Software or, alternatively, in accordance with the terms contained in
** a written agreement between you and The Qt Company. For licensing terms
** and conditions see https://www.qt.io/terms-conditions. For further
** information use the contact form at https://www.qt.io/contact-us.
**
** BSD License Usage
** Alternatively, you may use this file under the terms of the BSD license
** as follows:
**
** "Redistribution and use in source and binary forms, with or without
** modification, are permitted provided that the following conditions are
** met:
**   * Redistributions of source code must retain the above copyright
**     notice, this list of conditions and the following disclaimer.
**   * Redistributions in binary form must reproduce the above copyright
**     notice, this list of conditions and the following disclaimer in
**     the documentation and/or other materials provided with the
**     distribution.
**   * Neither the name of The Qt Company Ltd nor the names of its
**     contributors may be used to endorse or promote products derived
**     from this software without specific prior written permission.
**
**
** THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
** "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
** LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
** A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
** OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
** SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
** LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
** DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
** THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
** (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
** OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE."
**
** $QT_END_LICENSE$
**
****************************************************************************/
#include <QGuiApplication>
#include <QScreen>
#include <QSlider>
#include <QWidget>
#include <QApplication>
#include <QVBoxLayout>
#include "trianglewindow.h"
#include <QLabel>

int main(int argc, char **argv) {
    QGuiApplication app(argc, argv);

    QSurfaceFormat format;
    format.setSamples(16);

    TriangleWindow window;
    window.setFormat(format);
    window.resize(1600, 900);

    window.setAnimating(true);

    QApplication app2(argc, argv);
    QWidget window2;
    QVBoxLayout *layout = new QVBoxLayout(&window2);

    // Первый слайдер (Morph)
    QLabel *label1 = new QLabel("Morph:");
    QSlider slider(Qt::Horizontal);
    slider.setRange(0, 100);
    QObject::connect(&slider, &QSlider::valueChanged, &window,
                     &TriangleWindow::setUniform);

    // Второй слайдер (N)
    QLabel *label2 = new QLabel("N:");
    QSlider slider2(Qt::Horizontal);
    slider2.setRange(2, 100);
    QObject::connect(&slider2, &QSlider::valueChanged, &window,
                     &TriangleWindow::setN);

    // Добавление слайдеров и надписей на форму
    layout->addWidget(label1);
    layout->addWidget(&slider);
    layout->addWidget(label2);
    layout->addWidget(&slider2);

    window2.resize(200, 200);
    window2.setLayout(layout);
    window2.show();
    window.show();

    return app.exec();
}
