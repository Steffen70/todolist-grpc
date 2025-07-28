#include <todo_qt_client/greeter_client.hpp>

#include <QtWidgets/QApplication>
#include <QtWidgets/QLineEdit>
#include <QtWidgets/QPushButton>
#include <QtWidgets/QLabel>
#include <QtWidgets/QHBoxLayout>

#include <fmt/core.h>
#include <unistd.h>
#include <climits>

static std::string getExecutableDir()
{
    char buf[PATH_MAX];
    if (const ssize_t len = readlink("/proc/self/exe", buf, sizeof(buf) - 1); len != -1)
    {
        buf[len] = '\0';
        const std::string exePath(buf);
        return exePath.substr(0, exePath.find_last_of('/'));
    }
    throw std::runtime_error("Cannot determine executable path");
}

class MainWindow final : public QWidget
{
    // Make moc generate meta-object code
    Q_OBJECT

public:
    explicit MainWindow(QWidget* parent = nullptr)
        : QWidget(parent),
          _input(new QLineEdit(this)),
          _reply(new QLabel(this)),
          _button(new QPushButton("Say hello", this)),
          _client("localhost:8445", getExecutableDir() + "/root_ca.crt")
    {
        _input->setPlaceholderText("Enter your name");
        _reply->setTextInteractionFlags(Qt::TextSelectableByMouse);

        auto* row = new QHBoxLayout;
        row->addWidget(_input);
        row->addWidget(_button);

        auto* layout = new QVBoxLayout(this);
        layout->addLayout(row);
        layout->addWidget(_reply);

        connect(_button, &QPushButton::clicked,
                this, &MainWindow::onSayHelloClicked);
        connect(_input, &QLineEdit::returnPressed,
                _button, &QPushButton::click);
    }

private slots:
    void onSayHelloClicked() const
    {
        const QString name = _input->text().trimmed();
        if (name.isEmpty())
        {
            _reply->setText("Please enter a name first.");
            return;
        }
        try
        {
            const std::string result = _client.sayHello(name.toStdString());
            _reply->setText(QString::fromStdString(result));
        }
        catch (const std::exception& e)
        {
            _reply->setText(QStringLiteral("RPC failed: ") + e.what());
        }
    }

private:
    QLineEdit* _input;
    QLabel* _reply;
    QPushButton* _button;
    GreeterClient _client;
};

int main(int argc, char** argv)
{
    QApplication app(argc, argv);

    MainWindow w;
    w.setWindowTitle("Greeter Qt Client");
    w.resize(400, 120);
    w.show();

    return QApplication::exec();
}

// required because MainWindow is defined in a .cpp
#include "main.moc"
