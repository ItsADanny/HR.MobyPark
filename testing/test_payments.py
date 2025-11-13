import requests
import utilities

def test_register_createUser():
    r = requests.post("http://localhost:8000/register", json={"username": utilities.gen_username(), "password": utilities.gen_password()})
    assert r.status_code == 201
    assert r.text == "User created"


def test_Get_Payment_data_success():
    r = requests.post("http://localhost:8000/login", json={'username': utilities.get_saved_username(), 'password': utilities.get_saved_password()})
    assert r.status_code == 200
    assert r.text.startswith('{"message": "User logged in", "session_token": "')

    token = r.json()['session_token']

    rPayment = requests.get("http://localhost:8000/payments", headers={'Authorization': token})
    assert rPayment.status_code == 200
    data = rPayment.json()
    assert isinstance(data, list)

def test_Get_Payment_data_failure():
    rPayment = requests.get("http://localhost:8000/payments", headers={'Authorization': 'invalid_token'})
    assert rPayment.status_code == 401
    assert rPayment.text == "Unauthorized: Invalid or missing session token"


def test_Post_Payment_data_success():
    r = requests.post("http://localhost:8000/login", json={'username': utilities.get_saved_username(), 'password': utilities.get_saved_password()})
    assert r.status_code == 200
    assert r.text.startswith('{"message": "User logged in", "session_token": "')

    token = r.json()['session_token']
    
    payment_data = {
        "transaction": "test-transaction-123",
        "amount": 42.50
    }

    rPayment = requests.post("http://localhost:8000/payments", headers={'Authorization': token}, json=payment_data)
    assert rPayment.status_code == 201
    data = rPayment.json()
    assert data["status"] == "Success"
    assert data["payment"]["transaction"] == payment_data["transaction"]
    assert data["payment"]["amount"] == payment_data["amount"]

def test_Post_Payment_data_failure():

    rPayment = requests.post("http://localhost:8000/payments", headers={'Authorization': 'invalid_token'}, json={})
    assert rPayment.status_code == 401
    assert rPayment.text == "Unauthorized: Invalid or missing session token"

def test_update_payment_success():
    import time

    r = requests.post("http://localhost:8000/login", json={'username': utilities.get_saved_username(), 'password': utilities.get_saved_password()})
    assert r.status_code == 200
    assert r.text.startswith('{"message": "User logged in", "session_token": "')

    token = r.json()['session_token']
    unique_id = f"test-transaction-{int(time.time())}"
    payment_data = {
        "transaction": unique_id,
        "amount": 75.00
    }
    # POST
    rPayment = requests.post("http://localhost:8000/payments",headers={'Authorization': token},json=payment_data)
    assert rPayment.status_code == 201
    data = rPayment.json()

    payment_id = data["payment"]["transaction"]
    payment_hash = data["payment"]["hash"]

    # print("DEBUG: Payment created")
    # print("Payment ID:", payment_id)
    # print("Payment hash from response:", payment_hash)
    # print("Full payment data:", data["payment"])
    # PUT
    update_data = {
        "t_data": {"confirmation_code": "CONF12345"},
        "hash": payment_hash
    }
    rUpdate = requests.put(
        f"http://localhost:8000/payments/{payment_id}",
        headers={'Authorization': token},
        json=update_data
    )
    if rUpdate.status_code != 200:
        print("DEBUG: Update failed")
        print("Sent update_data:", update_data)
        print("Server response:", rUpdate.status_code, rUpdate.text)

    assert rUpdate.status_code == 200
    updated_payment = rUpdate.json()["payment"]

    assert "completed" in updated_payment
    assert updated_payment["t_data"]["confirmation_code"] == "CONF12345"
    assert updated_payment["hash"] == payment_hash


def test_update_payment_failure():
    # Step 1: Log in and get a valid session token
    r = requests.post(
        "http://localhost:8000/login",
        json={'username': utilities.get_saved_username(), 'password': utilities.get_saved_password()}
    )
    assert r.status_code == 200
    assert r.text.startswith('{"message": "User logged in", "session_token": "')
    token = r.json()['session_token']

    # Step 2: Prepare invalid payment ID and bad hash
    invalid_payment_id = "nonexistent-id"
    update_data = {
        "t_data": {"confirmation_code": "CONF12345"},
        "hash": "invalidhash"
    }

    # Step 3: Try to update a non-existing payment
    rUpdate = requests.put(
        f"http://localhost:8000/payments/{invalid_payment_id}",
        headers={'Authorization': token},
        json=update_data
    )

    # Step 4: Validate that the server responds correctly
    assert rUpdate.status_code == 404
    assert "Payment not found" in rUpdate.text
