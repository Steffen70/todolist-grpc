#include <string>

class Test {
public:
    Test(const std::string& name, const char& resourceA, const char& resourceB);
    ~Test();

    void say_hello() const;

private:
    std::string name_;
    int resourceA_;
    int resourceB_;
};
