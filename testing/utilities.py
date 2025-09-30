import random
from password_generator import PasswordGenerator
from storage_utils import load_json, write_json

firstname = [
    "Jan", "Pieter", "Kees", "Bram", "Sander",
    "Daan", "Luuk", "Tijs", "Willem", "Jeroen",
    "Thijs", "Lars", "Ruben", "Joost", "Maarten",
    "Femke", "Sanne", "Lieke", "Anouk", "Marloes",
    "Esmee", "Noa", "Sophie", "Tess", "Eva",
    "Lisa", "Floor", "Julia", "Roos", "Mila",
    "Nina", "Ilse", "Lotte", "Emma", "Fleur",
    "Anne", "Yara", "Loes", "Inge", "Merel",
    "Aart", "Barend", "Gerrit", "Hendrikus", "Theodoor",
    "Cornelis", "Lubbert", "Dirck", "Willehad", "Reinier",
    "Govert", "Tjebbe", "IJsbrand", "Fransiscus", "Berend",
    "Trijntje", "Geesje", "Neeltje", "Jannigje", "Antje",
    "Aaltje", "Lijsbeth", "Hendrina", "Willemina", "Maartje",
    "Geertruida", "Grietje", "Sijtske", "Dieuwertje", "Marijtje",
    "Engelbertus", "Adelgonda", "Beatrix", "Rutger", "Kornelis",
    "Joostina", "Catharina", "Pieternella", "Arent", "Feije",
    "Arthur", "George", "Edward", "Alfred", "Harold",
    "Frederick", "Rupert", "Reginald", "Nigel", "Graham",
    "Clive", "Percival", "Hugh", "Basil", "Cedric",
    "Beatrice", "Edith", "Winifred", "Agatha", "Enid",
    "Mabel", "Gwendolyn", "Clementine", "Ethel", "Millicent",
    "Rosamund", "Sybil", "Iris", "Muriel", "Hyacinth",
    "Lionel", "Malcolm", "Trevor", "Neville", "Alistair",
    "Imogen", "Pippa", "Tamsin", "Philippa", "Harriet",
    "Dafydd", "Owain", "Rhys", "Gruffudd", "Iwan",
    "Llewelyn", "Gethin", "Aneurin", "Emrys", "Carwyn",
    "Brynmor", "Hywel", "Ioan", "Tudur", "Maredudd",
    "Carys", "Eira", "Rhiannon", "Bronwen", "Ffion",
    "Seren", "Arianwen", "Angharad", "Eluned", "Lowri",
    "Nerys", "Tegan", "Blodwen", "Gwenno", "Meirion",
    "Eleri", "Myfanwy", "Anwen", "Catrin", "Siân",
    "Jackson", "Logan", "Hunter", "Wyatt", "Jayden",
    "Brayden", "Colton", "Maverick", "Jace", "Easton",
    "Chase", "Dakota", "Ryder", "Tanner", "Cody",
    "Madison", "Brooklyn", "Addison", "Kaylee", "Hailey",
    "Savannah", "Skylar", "Paisley", "Rylee", "Avery",
    "Peyton", "Kinsley", "Presley", "Reagan", "Harper",
    "Bentley", "Braxton", "Knox", "Grayson", "Ember",
    "Ashlyn", "Brielle", "Kenzie", "Jaxon", "Zayden"
]

lastname = [
    "Jansen", "De Vries", "Van den Berg", "Bakker", "Visser",
    "Smit", "Meijer", "De Boer", "Mulder", "De Groot",
    "Bos", "Vos", "Peters", "Hendriks", "Dekker", "Kok",
    "Jacobs", "Maas", "Koning", "Schouten", "Willems",
    "Post", "Van Dijk", "Hermans", "Kuiper", "Hoekstra",
    "Van Leeuwen", "Van der Meer", "Vermeulen", "Scholten",
    "Van Dam", "Van der Linden", "Prins", "Timmermans",
    "Boersma", "Veenstra", "Van Vliet", "Van der Heijden",
    "Smits", "Van den Broek", "De Lange", "Van der Veen",
    "Dijkstra", "Peeters", "Van den Heuvel", "Van Beek",
    "Van Wijk", "De Wit", "Heemskerk", "Van der Wal",
    "Smith", "Johnson", "Williams", "Brown", "Jones",
    "Miller", "Davis", "Garcia", "Rodriguez", "Martinez",
    "Harris", "Clark", "Lewis", "Robinson", "Walker",
    "Young", "Allen", "King", "Wright", "Scott",
    "Green", "Baker", "Adams", "Nelson", "Hill",
    "Campbell", "Mitchell", "Roberts", "Carter", "Phillips",
    "Turner", "Parker", "Evans", "Edwards", "Collins",
    "Stewart", "Morris", "Rogers", "Reed", "Cook",
    "Morgan", "Bell", "Murphy", "Bailey", "Rivera",
    "Cooper", "Richardson", "Cox", "Howard", "Ward",
    "Smith", "Johnson", "Williams", "Brown", "Jones",
    "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
    "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
    "Thomas", "Taylor", "Moore", "Jackson", "Martin",
    "Lee", "Perez", "Thompson", "White", "Harris",
    "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
    "Walker", "Young", "Allen", "King", "Wright",
    "Scott", "Torres", "Nguyen", "Hill", "Flores",
    "Green", "Adams", "Nelson", "Baker", "Hall",
    "Rivera", "Campbell", "Mitchell", "Carter", "Roberts"
]

def gen_username():
    selectedFirst = firstname[random.randint(0, len(firstname) - 1)]
    selectedLast = lastname[random.randint(0, len(lastname) - 1)]
    selectedLuckyNumbers = random.randint(0, 100000)

    username = f"{selectedFirst}{selectedLast}{selectedLuckyNumbers}"
    save_username(username)

    return username

def gen_name():
    selectedFirst = firstname[random.randint(0, len(firstname) - 1)]
    selectedLast = lastname[random.randint(0, len(lastname) - 1)]

    name = f"{selectedFirst} {selectedLast}"
    save_name(name)

    return name

def gen_password(length=20):
    pwo = PasswordGenerator()
    pwo.minlen = length
    pwo.maxlen = length

    password = pwo.generate()
    save_password(password)

    return password

def get_lastInsertedUser():
    users = load_json('data/users.json')
    return users[len(users) - 1].get("username")

def save_username(username):
    tempStorage = load_json('testing/temp_storage.json')

    tmpName = tempStorage.get("name")
    tmpPass = tempStorage.get("password")

    tempStorage["username"] = username
    write_json('testing/temp_storage.json', {"username": username, "password": tmpPass, "name": tmpName})

def save_password(password):
    tempStorage = load_json('testing/temp_storage.json')

    tmpUsername = tempStorage.get("username")
    tmpName = tempStorage.get("name")

    write_json('testing/temp_storage.json', {"username": tmpUsername, "password": password, "name": tmpName})

def save_name(name):
    tempStorage = load_json('testing/temp_storage.json')

    tmpUsername = tempStorage.get("username")
    tmpPassword = tempStorage.get("password")

    write_json('testing/temp_storage.json', {"username": tmpUsername, "password": tmpPassword, "name": name})

def get_saved_username():
    tempStorage = load_json('testing/temp_storage.json')
    return tempStorage.get("username")

def get_saved_password():
    tempStorage = load_json('testing/temp_storage.json')
    return tempStorage.get("password")

def get_saved_name():
    tempStorage = load_json('testing/temp_storage.json')
    return tempStorage.get("name")