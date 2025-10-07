#Library used for making the request to the endpoints
import requests
import utilities

def test_register_createUser():
    r = requests.post("http://localhost:8000/register", json={"username": utilities.gen_username(), "password": utilities.gen_password()})
    assert r.status_code == 201
    assert r.text == "User created"

def test_register_usernameTaken():
    r = requests.post("http://localhost:8000/register", json={'username': utilities.get_lastInsertedUser(), 'password': 'ThisIsntImportant'})
    assert r.status_code == 200
    assert r.text == "Username already taken"

# NOTE: (ItsDanny - Danny - HRNumber: 1091749) [30-09-2025]
# THIS IS AN SUGGESTION FOR IMPROVEMENT OF THE POSSIBLE RESPONSES
# Adding minimal requirements for password length, we can now add passwords that are 1 character long
def test_register_shortPassword():
    r = requests.post("http://localhost:8000/register", json={'username': utilities.gen_username(), 'password': utilities.gen_password(1)})
    assert r.status_code == 201
    assert r.text == "User created"

def test_register_longPassword():
    r = requests.post("http://localhost:8000/register", json={'username': utilities.gen_username(), 'password': utilities.gen_password(100)})
    assert r.status_code == 201
    assert r.text == "User created"

def test_login_succes():
    r = requests.post("http://localhost:8000/login", json={'username': utilities.get_saved_username(), 'password': utilities.get_saved_password()})
    assert r.status_code == 200
    assert r.text.startswith('{"message": "User logged in", "session_token": "')

def test_login_fail_missingCredentials():
    r = requests.post("http://localhost:8000/login", json={})
    assert r.status_code == 400
    assert r.text == "Missing credentials"

def test_login_fail_notExistingUser():
    r = requests.post("http://localhost:8000/login", json={'username': 'StarFruitMan', 'password': 'AppleCannons'})
    assert r.status_code == 401
    assert r.text == "User not found"

def test_login_fail_wrongPassword():
    r = requests.post("http://localhost:8000/login", json={'username': utilities.get_saved_username(), 'password': 'OrangeCannons'})
    assert r.status_code == 401
    assert r.text == "Invalid credentials"

def test_logout_succes():
    rLogin = requests.post("http://localhost:8000/login", json={'username': utilities.get_saved_username(), 'password': utilities.get_saved_password()})
    assert rLogin.status_code == 200
    assert rLogin.text.startswith('{"message": "User logged in", "session_token": "')
    token = rLogin.json()['session_token']

    rLogout = requests.get("http://localhost:8000/logout", headers={'Authorization': token})
    assert rLogout.status_code == 200
    assert rLogout.text == "User logged out"

def test_logout_fail_InvalidToken():
    r = requests.get("http://localhost:8000/logout", headers={'Authorization': ''})
    assert r.status_code == 400
    assert r.text == "Invalid session token"

# NOTE: (ItsDanny - Danny - HRNumber: 1091749) [29-09-2025]
# THIS IS AN SUGGESTION FOR IMPROVEMENT OF THE POSSIBLE RESPONSES
# This one will do the same as the one above, but we want to add an extra response to show
# that the token hasn't been put into the headers
def test_logout_fail_NoTokenGiven():
    r = requests.get("http://localhost:8000/logout")
    assert r.status_code == 400
    assert r.text == "Invalid session token"

def test_billing_fail_noToken():
    r = requests.get("http://localhost:8000/billing")
    assert r.status_code == 401
    assert "Unauthorized" in r.text



def test_billing_fail_invalidToken():
    r = requests.get("http://localhost:8000/billing", headers={'Authorization': 'invalidtoken123'})
    assert r.status_code == 401
    assert "Unauthorized" in r.text

#Er komt hier nog een laatste test 


def test_parking_lots_get_all():
    r = requests.get("http://localhost:8000/parking-lots")
    assert r.status_code == 200
    data = r.json()
    assert isinstance(data, dict)
    assert len(data) > 0
    first_key = next(iter(data))
    first_lot = data[first_key]
    assert "id" in first_lot
    assert "name" in first_lot
    assert "location" in first_lot
    assert "capacity" in first_lot


def test_parking_lot_by_id():
    r = requests.get("http://localhost:8000/parking-lots/10")
    assert r.status_code == 200
    lot = r.json()
    assert lot["id"] == "10"
    assert "name" in lot
    assert "tariff" in lot
    assert "address" in lot


def test_parking_lot_not_found():
    r = requests.get("http://localhost:8000/parking-lots/9999")
    assert r.status_code in [400, 404]
    assert "not found" in r.text.lower()
