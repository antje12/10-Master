import numpy as np
import statsmodels.stats.multicomp as multi

with open("Json_results.txt", "r") as file:
    json = np.array([float(line.strip()) for line in file.readlines()])

with open("Avro_results.txt", "r") as file:
    avro = np.array([float(line.strip()) for line in file.readlines()])

with open("Protobuf_results.txt", "r") as file:
    proto = np.array([float(line.strip()) for line in file.readlines()])

values = np.concatenate([json, avro, proto])
groups = np.array(['Json']*len(json) + ['Avro']*len(avro) + ['Protobuf']*len(proto))

tukey_results = multi.pairwise_tukeyhsd(values, groups, 0.05)
print(tukey_results)
