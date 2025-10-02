import requests
import utilities

def test_Get_Payment_data():
    r = requests.post("http://localhost:8000/login", json={'username': utilities.get_saved_username(), 'password': utilities.get_saved_password()})
    assert r.status_code == 200
    assert r.text.startswith('{"message": "User logged in", "session_token": "')

    token = r.json()['session_token']

    rPayment = requests.get("http://localhost:8000/payments", headers={'Authorization': token})
    assert rPayment.status_code == 200
    data = rPayment.json()
    assert isinstance(data, list)


def test_Post_Payment_data():
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