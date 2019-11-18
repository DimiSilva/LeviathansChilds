using System;

public class User
{
    public Guid id { get; private set; }
    public string fisrtName { get; private set; }
    public string lastName { get; private set; }
    public string nick { get; private set; }
    public string emailAdress { get; private set; }
    public string password { get; private set; }
    public string status { get; private set; }


    public User(Guid id, string fisrtName, string lastName, string nick, string emailAdress, string password)
    {
        this.id = id;
        this.fisrtName = fisrtName;
        this.lastName = lastName;
        this.nick = nick;
        this.emailAdress = emailAdress;
        this.password = password;
    }

    public User(string id, string firstName, string emailAdress)
    {
        this.id = Guid.Parse(id);
        this.fisrtName = firstName;
        this.emailAdress = emailAdress;
    }
}