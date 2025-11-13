import requests
import utilities

def login_user_and_get_token(): # Helper function
    username = utilities.get_saved_username()
    password = utilities.get_saved_password()
    r = requests.post("http://localhost:8000/login", json={"username": username, "password": password})
    assert r.status_code == 200
    return r.json()["session_token"]

# def test_get_all_parking_lots():
#     token = login_user_and_get_token()
#     r = requests.get("http://localhost:8000/parking-lots", headers={'Authorization': token})
#     assert r.status_code == 200
#     data = r.json()
#     assert isinstance(data, dict)
#     assert len(data) > 0
#     first_key = next(iter(data))
#     first_lot = data[first_key]
#     assert "id" in first_lot
#     assert "name" in first_lot
#     assert "location" in first_lot
#     assert "capacity" in first_lot

def test_get_parking_lot_by_id():
    token = login_user_and_get_token()
    r = requests.get("http://localhost:8000/parking-lots/10", headers={'Authorization': token})
    assert r.status_code == 200
    lot = r.json()
    assert lot["id"] == "10"
    assert "name" in lot
    assert "tariff" in lot
    assert "address" in lot

def test_parking_lot_not_found():
    r = requests.get("http://localhost:8000/parking-lots/1600")
    assert r.status_code in [400, 404]
    assert "not found" in r.text.lower()

def test_start_parking_session():
    token = login_user_and_get_token()
    payload = {"licenseplate": "TEST123"}
    r = requests.post("http://localhost:8000/parking-lots/1/sessions/start", headers={'Authorization': token}, json=payload)
    assert r.status_code == 200
    assert f"Session started for: {payload['licenseplate']}" in r.text

def test_start_parking_session_missing_licenseplate_fail():
    token = login_user_and_get_token()
    r = requests.post("http://localhost:8000/parking-lots/1/sessions/start", headers={'Authorization': token}, json={})
    assert r.status_code == 401
    assert "Require field missing" in r.text

def test_stop_parking_session():
    token = login_user_and_get_token()
    payload = {"licenseplate": "TEST123"}
    r_start = requests.post("http://localhost:8000/parking-lots/1/sessions/start", headers={'Authorization': token}, json=payload)
    assert r_start.status_code == 200

    r_stop = requests.post("http://localhost:8000/parking-lots/1/sessions/stop", headers={'Authorization': token}, json=payload)
    assert r_stop.status_code == 200
    assert f"Session stopped for: {payload['licenseplate']}" in r_stop.text

def test_stop_parking_session_no_active_session_fail():
    token = login_user_and_get_token()
    payload = {"licenseplate": "NOSESSION123"}
    r = requests.post("http://localhost:8000/parking-lots/1/sessions/stop", headers={'Authorization': token}, json=payload)
    assert r.status_code == 400
    assert "Cannot stop a session when there is no session for this licesenplate." in r.text