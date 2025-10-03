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

# def test_update_payment_success():

#     r = requests.post("http://localhost:8000/login", json={'username': utilities.get_saved_username(), 'password': utilities.get_saved_password()})
#     assert r.status_code == 200
#     assert r.text.startswith('{"message": "User logged in", "session_token": "')

#     token = r.json()['session_token']
    
#     payment_data = {
#         "transaction": "test-transaction-456",
#         "amount": 75.00
#     }
#     # POST
#     rPayment = requests.post("http://localhost:8000/payments",headers={'Authorization': token},json=payment_data)
#     assert rPayment.status_code == 201
#     data = rPayment.json()

#     payment_id = data["payment"]["transaction"]
#     payment_hash = data["payment"]["hash"]

#     print("DEBUG: Payment created")
#     print("Payment ID:", payment_id)
#     print("Payment hash from response:", payment_hash)
#     print("Full payment data:", data["payment"])
#     # PUT
#     update_data = {
#         "t_data": {"confirmation_code": "CONF12345"},
#         "hash": payment_hash
#     }
#     rUpdate = requests.put(
#         f"http://localhost:8000/payments/{payment_id}",
#         headers={'Authorization': token},
#         json=update_data
#     )
#     if rUpdate.status_code != 200:
#         print("DEBUG: Update failed")
#         print("Sent update_data:", update_data)
#         print("Server response:", rUpdate.status_code, rUpdate.text)

    # assert rUpdate.status_code == 200
    # updated_payment = rUpdate.json()["payment"]

    # assert "completed" in updated_payment
    # assert updated_payment["t_data"]["confirmation_code"] == "CONF12345"
    # assert updated_payment["hash"] == payment_hash

# [
#   {
#     "transaction": "1535349fea5cca288b217d491838f836AA",
#     "amount": 5.5,
#     "initiator": "testuser",
#     "created_at": "22-05-2025 09:09:1747898315",
#     "completed": "22-05-2025 09:09:1747898330",
#     "hash": "d15acc14-02c3-4c9d-b047-c6b16befc302",
#     "t_data": {
#       "amount": 5.5,
#       "date": "2025-05-22 22:22:22",
#       "method": "ideal",
#       "issuer": "XYY910HH",
#       "bank": "ABN-NL"
#     }
#   },
#   {
#     "transaction": "82724346a21e6ddf8da490f7c49355bcBB",
#     "amount": 16.5,
#     "initiator": "natasjadewit",
#     "created_at": "26-03-2020 05:40:151585197647",
#     "completed": "26-03-2020 05:40:151585197655",
#     "hash": "3524ea52-c49a-4a5b-9395-82f2a152368f",
#     "t_data": {
#       "amount": 16.5,
#       "date": "2020-03-26 05:40:47",
#       "method": "bancontact",
#       "issuer": "9I2G36YZ",
#       "bank": "Bunq"
#     },
#     "session_id": "1",
#     "parking_lot_id": "1"
#   },
#   {
#     "transaction": "09b825af4025bd30be494b444d386b35AA",
#     "amount": 16.97,
#     "initiator": "cindy.adriaanse1",
#     "created_at": "26-03-2020 21:20:151585254012",
#     "completed": "26-03-2020 21:20:151585254020",
#     "hash": "d4614f50-0f03-40e6-96eb-f8ca0374fbaa",
#     "t_data": {
#       "amount": 16.97,
#       "date": "2020-03-26 21:20:12",
#       "method": "ideal",
#       "issuer": "E0M9P5UG",
#       "bank": "Knab"
#     }
#   }
# ]