# This script is used to generate the random data for the machines, settings, objects, and properties

import random
import json

class Machine:
    def __init__(self, machine_id, title):
        self.machine_id = machine_id
        self.title = title

class Setting:
    def __init__(self, machine_id, setting_id, title, min_value, max_value):
        self.machine_id = machine_id
        self.setting_id = setting_id
        self.title = title
        self.min_value = min_value
        self.max_value = max_value

class Property:
    def __init__(self, title, min_value=0, max_value=100):
        self.title = title
        self.min_value = min_value
        self.max_value = max_value

class Object:
    def __init__(self, object_id, title, settings):
        self.object_id = object_id
        self.title = title
        self.settings = {setting.setting_id: random.randint(setting.min_value, setting.max_value) for setting in settings}

class Game:
    def __init__(self):
        self.machines = []
        self.settings = []
        self.objects = []
        self.properties = []

    def add_machine(self, machine_id, title):
        self.machines.append(Machine(machine_id, title))

    def add_setting(self, machine_id, setting_id, title, min_value, max_value):
        self.settings.append(Setting(machine_id, setting_id, title, min_value, max_value))

    def add_property(self, title, min_value=0, max_value=100):
        self.properties.append(Property(title, min_value, max_value))

    def add_object(self, object_id, title):
        all_setting_ids = [setting.setting_id for setting in self.settings]
        settings = [setting for setting in self.settings if setting.setting_id in all_setting_ids]
        self.objects.append(Object(object_id, title, settings))

    def generate_covariance_matrix(self):
        n_properties = len(self.properties)
        n_settings = len(self.settings)
        return [[random.randint(0, 1) for _ in range(n_settings)] for _ in range(n_properties)]

    def to_json(self):
        covariance_matrix = self.generate_covariance_matrix()
        return json.dumps({
            "machines": [{"machine_id": m.machine_id, "title": m.title} for m in self.machines],
            "settings": [{"machine_id": s.machine_id, "setting_id": s.setting_id, "title": s.title, "min": s.min_value, "max": s.max_value} for s in self.settings],
            "properties": [{"title": p.title, "min": p.min_value, "max": p.max_value} for p in self.properties],
            "covariance_matrix": covariance_matrix,
            "objects": [{"object_id": o.object_id, "title": o.title, "settings": [{"key": k, "value": v} for k, v in o.settings.items()]} for o in self.objects],
        }, indent=4)




game = Game()

# Add machines and settings
game.add_machine("m1","Slicer")
game.add_setting("m1", "s1", "Speed", 1, 10)
game.add_setting("m1", "s2", "Force", 1, 25)

game.add_machine("m2","Gluer")
game.add_setting("m2", "s3", "Feed rate", 2, 8)
game.add_setting("m2", "s4", "Temperature", 25, 250)
game.add_setting("m2", "s5", "Pressure", 20, 40)

# Add game objects
game.add_object(f"o1", f"First run")
game.add_object(f"o2", f"Second")
game.add_object(f"o3", f"Tres")
game.add_object(f"o4", f"Vier")
game.add_object(f"o5", f"Five")
game.add_object(f"o6", f"Sechs")
game.add_object(f"o7", f"Siete")
game.add_object(f"o8", f"Ocho")
game.add_object(f"o9", f"Nueve")

# Add object properties. These need to be the same as defined in Blender for the object.
game.add_property("Height")
game.add_property("Width")
game.add_property("Thickness")
game.add_property("Smoothness")
game.add_property("Warped")
game.add_property("Bulge")

# Print JSON data
json_data = game.to_json()
print(json_data)

# Write JSON data to file
with open('data.json', 'w') as json_file:
    json_file.write(json_data)
