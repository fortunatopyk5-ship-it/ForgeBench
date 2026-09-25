# References / source ledger

Primary/classical:
- A. Jaffe, E. Witten, "Quantum Yang-Mills Theory", official Clay Millennium problem description.
- K. Osterwalder, R. Schrader, "Axioms for Euclidean Green's Functions II", Commun. Math. Phys. 42 (1975) 281-305. Corrected reconstruction theorem.
- M. Luscher, "Construction of a selfadjoint, strictly positive transfer matrix for Euclidean lattice gauge theories", Commun. Math. Phys. 54 (1977) 283-292.
- K. Osterwalder, E. Seiler, "Gauge Field Theories on a Lattice", Ann. Phys. 110 (1978) 440-471.
- T. Balaban, series on UV stability / RG for lattice gauge theories, 1980s.

Functional inequalities / strong coupling:
- H. Shen, R. Zhu, X. Zhu, "A stochastic analysis approach to lattice Yang-Mills at strong coupling", Commun. Math. Phys. 400 (2023) 805-851; arXiv:2204.12737.
  Key formulas tracked: SU(N) strong-coupling condition |beta|<1/[16(d-1)]; Poincare/LSI; covariance decay; proof combines PI semigroup decay with commutator quasi-locality.

Current overview:
- M. R. Douglas, "The Yang-Mills Millennium problem", Nature Reviews Physics 8 (2026) 86-97.

Audit policy:
Recent independent or AI-assisted claims of a complete proof are not accepted here merely from their abstracts/websites. Each claimed bridge must be re-derived and checked for cutoff/volume dependence, RP/OS reconstruction, nontriviality, and hidden hypotheses.


Constructive UV / fixed-IR control:
- J. Magnen, V. Rivasseau, R. Seneor, "Construction of YM4 with an infrared cutoff", Commun. Math. Phys. 155 (1993) 325-383. Pure SU(2), fixed IR cutoff, UV cutoff removed in a regularized axial-gauge construction.
- T. Balaban, "Large field renormalization I-II", Commun. Math. Phys. 122 (1989). Completes the stated ultraviolet-stability program for four-dimensional pure lattice gauge theory.

These results are treated as UV-side inputs only; they do not supply the cutoff/volume-uniform IR mass-gap estimate used in YM-CROSSOVER-1.


Additional crossover references checked in Iteration 2:
- T. Balaban, "Averaging operations for lattice gauge theories", Commun. Math. Phys. 98 (1985) 17-51. The abstract explicitly states that RG transformations are defined by averaging operations and studies regular/analytic gauge-field averaging maps.
- T. Balaban, "Propagators for lattice gauge theories in a background field", Commun. Math. Phys. 99 (1985) 389-434. The abstract states regularity and decay properties for RG propagators in external gauge backgrounds.
- T. Balaban, "Renormalization group approach to lattice gauge field theories. I", Commun. Math. Phys. 109 (1987) 249-301. Small-field effective actions and coupling renormalization in four dimensions.
- T. Balaban, "Renormalization group approach to lattice gauge field theories. II. Cluster expansions", Commun. Math. Phys. 116 (1988) 1-22. The fluctuation integral is represented by an exponentiated cluster expansion and the terms preserve the small-field inductive assumptions.
- T. Balaban, "Convergent renormalization expansions for lattice gauge theories", Commun. Math. Phys. 119 (1988) 243-285. Introduces inductive complete effective densities including large-field domains; the abstract notes convergent expansions for the superrenormalizable cases treated there.
- T. Balaban, "Large field renormalization. I-II", Commun. Math. Phys. 122 (1989). Part II states that its R-operation bounds complete the proof of ultraviolet stability of four-dimensional pure gauge theories.
- R. Kotecky, D. Preiss, "Cluster expansion for abstract polymer models", Commun. Math. Phys. 103 (1986) 491-498. Standard sufficient criterion used in YM-KP-ANCHOR-1.
- J. Dimock, "The renormalization group according to Balaban I-III" (2013-2014). Scalar expository model, useful for the polymer/tree-distance Banach-space architecture; NOT a source for a completed 4D Yang-Mills crossover.
- T. Balaban, M. O'Carroll, "A Simple Method for Correlation Functions via the Effective Actions in the Renormalization Group Framework", Ann. Phys. 260 (1997) 1-8. Relevant to the source/observable issue: correlation functions can be recovered from source-dependent effective actions when the required effective-action bounds are available.

Source-discipline note:
The concrete Banach space and heat-kernel exact RG in crossover_spec.md are proposed research definitions, not claims that they are Balaban's exact original definitions.
