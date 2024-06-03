include(openglwindow.pri)

SOURCES += \
    Edge.cpp \
    main.cpp \
    trianglewindow.cpp

INSTALLS += target
QT += widgets

DISTFILES += \
    fragmentShader.fsh \
    shader.vsh

HEADERS += \
    Edge.h \
    trianglewindow.h
