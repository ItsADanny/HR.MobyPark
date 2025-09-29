#Library used for making the request to the endpoints
import requests
import json

def test_register_createUser():
    r = requests.post("http://localhost:8000/register", json={"username": "AppleMan", "password": "CarrotCannon"})
    assert r.status_code == 201
    assert r.text == "User created"

def test_register_usernameTaken():
    r = requests.post("http://localhost:8000/register", json={'username': 'AppleMan', 'password': 'CarrotCannon'})
    assert r.status_code == 200
    assert r.text == "Username already taken"

def test_login_succes():
    r = requests.post("http://localhost:8000/login", json={"username": "cindy.leenders42", "password": "password102"})
    assert r.status_code == 200

def test_login_missingCredentials():
    r = requests.get("http://localhost:8000/login")
    assert r.status_code == 400
    assert r.text == "Missing credentials"

def test_login_InvalidCrendentials():
    r = requests.get("http://localhost:8000/login")
    assert r.status_code == 401
    assert r.text == "Invalid credentials"

def test_logout_succes():
    r1 = requests.post("http://localhost:8000/login")
    assert r1.status_code == 200

    data = r1.json()
    sessionToken = data["session_token"]

    r2 = requests.post("http://localhost:8000/logout", json={"session_token": sessionToken})
    assert r2.status_code == 200