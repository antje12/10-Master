import os
import numpy as np
import statsmodels.stats.multicomp as multi

def read_latencies_from_folder(folder_path):
    latencies = []
    # Loop over all CSV files in the folder
    for filename in os.listdir(folder_path):
        if filename.endswith(".csv"):
            file_path = os.path.join(folder_path, filename)
            with open(file_path, "r") as file:
                # Read each line, split by ';', and extract the latency value
                for line in file:
                    parts = line.strip().split(';')
                    if len(parts) > 1:  # Ensure there is a latency value
                        latencies.append(float(parts[1]))  # Assuming latency is the second part
    return np.array(latencies)

# Paths to the folders
folder1 = "1 Instances/50 Clients"
folder2 = "2 Instances/50 Clients"
folder3 = "3 Instances/50 Clients"

# Read latencies from each folder
latencies1 = read_latencies_from_folder(folder1)
latencies2 = read_latencies_from_folder(folder2)
latencies3 = read_latencies_from_folder(folder3)

# Concatenate values and create groups
values = np.concatenate([latencies1, latencies2, latencies3])
groups = np.array(['1Instance']*len(latencies1) + ['2Instances']*len(latencies2) + ['3Instances']*len(latencies3))

# Perform Tukey HSD test
tukey_results = multi.pairwise_tukeyhsd(values, groups, 0.05)
print(tukey_results)