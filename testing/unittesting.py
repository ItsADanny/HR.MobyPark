import pytest
import requests
#
def test_register():
    r = requests.post("http://localhost:8000/register", json={"username": "CarrotMan", "password": "CarrotCannon"})
    assert r.status_code == 201
    assert r.text == "User created"

    r2 = requests.post("http://localhost:8000/register", json={'username': 'CarrotMan', 'password': 'CarrotCannon'})
    assert r2.status_code == 200
    assert r2.text == "Username already taken"