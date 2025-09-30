import requests
import utilities

def test_flow_account_RegisterLoginLogout():
    # Flow:
    # Register new account -> Logging-in with the new account -> Logging-out with the account

    # Generate the username and password we are going to use for this test
    username = utilities.gen_username()
    password = utilities.gen_password()

    # make a register request to make a new user account
    rRegister = requests.post(
        "http://localhost:8000/register",
        json={"username": username, "password": password}
    )

    # Check to see if the result from our registration attempt are as expected
    assert rRegister.status_code == 201
    assert rRegister.text == "User created"

    # make the login request with the new account
    rLogin = requests.post(
        "http://localhost:8000/login",
        json={"username": username, "password": password}
    )
    # Check if the results for the Login endpoint are as expected
    assert rLogin.status_code == 200
    assert rLogin.text.startswith('{"message": "User logged in", "session_token": "')

    # Retrieve the session token from the response JSON
    token = rLogin.json()['session_token']

    # Testing the logout endpoint
    rLogout = requests.get(
        "http://localhost:8000/logout",
        headers={'Authorization': token}
    )

    # Check if the logout has been successful
    assert rLogout.status_code == 200
    assert rLogout.text == "User logged out"
