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


Iteration 3 source-extraction discipline:
- Publisher/metadata sources corroborate the paper-level scope: CMP109 is explicitly a small-field effective-action/coupling-renormalization paper; CMP116 develops the cluster-expansion step; CMP122-II states completion of the ultraviolet-stability program under its stated small-coupling framework.
- Equation-level CMP119/CMP122 formulas used in match_extract_audit.md were cross-checked against the public repository lluiseriksson/THE-ERIKSSON-PROGRAMME, whose source manifest records SHA-256-pinned local Balaban PDFs and marks the relevant transcriptions as visual_confirmed. These are treated here as SECONDARY TRANSCRIPTIONS, not direct primary-source verification by this notebook.
- In particular, no equation-level transcription is promoted to a theorem beyond its listed hypotheses/dictionary. The open source-to-our-topology conversion is exactly YM-MATCH-EXTRACT-1.

Iteration 3 extracted source shapes (secondary visual transcriptions):
- CMP119 (2.18): domain-history density representation with chi_k, T_k, exp A_k.
- CMP119 (2.23): A_k = -A(1/g_k^2,U_k)+E_k+R_k+B_k-mathcalE_k.
- CMP119 (2.24): 1/g_{j-1}^2(x)=1/g_j^2(x)+beta_j(g_{j-1}) phi_j(x).
- CMP119 (2.31): |R^(j)(X,(U,J))| <= g_j^kappa0 exp(-kappa d_j(X)).
- CMP119 (2.42): |B^(j)(X,...)| < B0 exp(-kappa d_j(X)).
- CMP122-I (1.70): localized large-field C-term bound C0 exp(-(1+3 beta)kappa d_m(X)).
- CMP122-II (1.99)-(1.100): localized post-R bounds, including an exp(-p0(g_k)) exp(-kappa d_k(X)) sector.
- CMP122-II Theorem 1: preservation of the CMP119 Sect.2 density form/conditions while all effective couplings remain in a sufficiently small interval.

These formulas justify the domain-indexed activity topology investigation; they do NOT justify a global all-field C^p matching ball.


Iteration 4 primary-source access audit:
- Rutgers repository records CMP109 and CMP119 as open version-of-record links, but the DOI links redirect to Springer endpoints that were inaccessible to the available browser in this run. Therefore no CMP109/CMP119 equation-level statement was newly upgraded to direct-primary-verified status.
- CMP122-II bibliographic/paper-level metadata and abstract were verified through Rutgers/publisher-indexed metadata, but equation/Theorem-1 bodies remain secondary visual transcriptions from the SHA-pinned source-audit ledger.
- The source-audit ledger marks CMP109 printed p.257 / PDF p.9 as source-extracted for the definition of d_j(X): shortest tree-graph length intersecting all localization cubes, divided by M. This is used only with that provenance label; it is not claimed as newly direct primary verification.
- CMP119's abstract-level statement that complete effective densities include large-field domains and that the renormalization transformations preserve their form is independently corroborated by Rutgers/OpenAIRE metadata. This is paper-level support, not an equation-level dictionary.


Iteration 5 DIRECT PRIMARY verification upgrade:
The actual article PDFs for CMP119 and CMP122-II were inspected directly page-by-page through accessible PDF mirrors. The following history/reconstruction claims are therefore upgraded from secondary visual transcription to DIRECT PRIMARY VERIFIED.

CMP119, T. Balaban, Commun. Math. Phys. 119 (1988) 243-285:
- printed p.246 Eq. (1.1): first raw hard large/small-field partition over P_0;
- printed p.246 Eq. (1.4): hard partition over P_1;
- printed p.247 Eq. (1.8): hard partition over Q_1;
- printed p.248 Eqs. (1.10)-(1.11): Omega_1 construction and resummation over admissible P_0,P_1,Q_1 into zeta(Omega_1^c);
- printed p.254 Eq. (2.1): Omega/Lambda interleaving
  Omega_1 superset Lambda_1 superset ... superset Omega_k superset Lambda_k;
- printed p.254-p.255: S_j subset Omega_j intersect Lambda_j^c, cube structure, nonempty/collar requirement, characteristic chi(Omega_j intersect Lambda_j^c,S_j), and zeta;
- printed p.255 Eqs. (2.2)-(2.3): determining strips and Z_j=Lambda_j^c;
- printed p.257 Eqs. (2.17)-(2.18): terminal chi_k(Omega_k), effective-density sum over admissible Omega/Lambda sequences, with S_j summation included in T_k;
- printed p.258 Eqs. (2.19)-(2.23): T_k component factorization, ordered one-step factorization, integral-operator structure, E/R/B action decomposition;
- printed p.259 Eq. (2.24): localized coupling recursion and phi_j in C_0^infinity(Lambda_j), phi_j=1 on the stated interior;
- printed p.265 Eqs. (3.2)-(3.5): exact hard P_{k+1},Q_{k+1} decompositions and Omega_{k+1} construction;
- printed p.267 text after (3.14): fixed-Omega_{k+1} resummation over admissible P_{k+1},Q_{k+1} into zeta(Omega_{k+1}^c);
- printed p.268 Eq. (3.16): exact hard R_{k+1} decomposition;
- printed p.269 Eqs. (3.20)-(3.21): Lambda_{k+1},S_{k+1}, and associated characteristic structure;
- printed p.270 Eqs. (3.23)-(3.24): explicit sum over S_{k+1} inside T^(k+1), and T_{k+1}=T^(k+1)T_k.

CMP122-II, T. Balaban, Commun. Math. Phys. 122 (1989) 355-392:
- printed p.380 Eq. (1.75): normalized local T'_k operator bound
  |(T'_k 1)^(-1)T'_k F| <= exp(3 sup|sigma|) sup|F|;
- printed pp.380-381: explicit negative large-field factors from P_j,Q_j,R_j and their componentwise combined p_0(g_j), p_0(g_j)^2 suppression;
- printed p.381: source statement that these are the fundamental large-field factors controlling convergence of the effective-density expansion;
- printed p.383 before Eq. (1.79): sums over admissible sequences are replaced by entropy factors exp{O(1)(MR_j)^(-d)|Z_j|};
- printed p.383 Eq. (1.79): combined positive history/integration cost and negative large-field penalties;
- printed pp.384-387 Eqs. (1.80)-(1.88): induction for component cost kappa_j(Z), including new exp[-p_0(g_j)] factors when new large fields are created;
- printed p.387 Eq. (1.89): clean local bound T'_k(X)1 <= exp[-2(1+beta_0)^(-1)p_0(g_k)];
- printed pp.390-391 Eqs. (1.100)-(1.102): post-R localized remainder and recovery of the CMP119 Sect.2 inductive density form;
- printed p.355 Theorem 1: under sufficiently small effective couplings, the densities have the form and satisfy the conditions/bounds of CMP119 Sect.2.

Iteration-5 source interpretation:
- raw characteristic decompositions are exact hard partitions;
- the final CMP119 Eq. (2.18) representation is compressed, so chi_k(Omega_k) is NOT a complete-history characteristic;
- history entropy is explicitly counted in CMP122-II and paid by large-field energy factors inside the T/R construction;
- the remaining YM-HISTORY-RECON-1 blocker is functional-analytic transport to a fixed volume-independent density/ambient Banach topology, not an unidentified source combinatorics mechanism.


Iteration 5 DIRECT PRIMARY CMP109 upgrade:
- T. Balaban, "Renormalization Group Approach to Lattice Gauge Field Theories. I", CMP 109 (1987) 249-301, printed p.257 / PDF p.8-9: a localization domain X is a connected finite union of pi_j-cubes and d_j(X) is the length (after the source normalization by M / unit-cube rescaling) of a shortest tree graph contained in X and intersecting every cube; the source notes the shortest tree can be taken along cube edges.
- This direct definition implies the uniform geometric coercivity used here: in d dimensions |X|<=2^d(d_j(X)+1), hence in d=4 |X|<=16(d_j(X)+1). Combined with maximum cube-adjacency degree 8 and canonical DFS encoding, it gives the explicit fixed-history shell bound recorded as YM-BALABAN-TREE-SHELL-COUNT-1.
- This closes polymer/tree entropy at fixed history. It does not replace the separate CMP122-II admissible-history entropy estimate.
