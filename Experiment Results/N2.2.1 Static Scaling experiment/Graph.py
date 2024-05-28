import pandas as pd
import matplotlib.pyplot as plt
import os

# Define the root directory where folders are located
root_dir = os.getcwd()

# Initialize a dictionary to store data
data_dict = {}

# Loop through each folder in the root directory
for folder in os.listdir(root_dir):
    folder_path = os.path.join(root_dir, folder)
    if os.path.isdir(folder_path):
        # Construct the path to the CSV file
        file_path = os.path.join(folder_path, 'Results.csv')
        if os.path.exists(file_path):
            # Read the CSV file
            df = pd.read_csv(file_path, sep=';', header=None, names=['clients', 'latency'])
            # Store the dataframe in the dictionary with folder name as key
            data_dict[folder] = df

# Plotting
plt.figure(figsize=(10, 5))
for label, df in data_dict.items():
    plt.plot(df['clients'], df['latency'], label=label)

# Add a horizontal line at 150 ms
plt.axhline(y=150, color='blue', linestyle='--', label='Threshold at 150 ms')

plt.xlabel('Concurrent Clients')
plt.ylabel('Latency (ms)')
plt.title('Performance Comparison')
plt.legend()
plt.grid(True)
plt.show()
