using System;
using System.Collections.Generic;

namespace TPClient.APIs
{
    public interface IJokeAPI
    {
        string Name { get; }

        List<string> Categories { get; }

        string GetJoke();
    }
}
