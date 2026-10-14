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


Iteration 6 direct-primary/local-norm extraction:
- CMP109, printed p.257 around Eqs. (0.25)-(0.26): after defining the shortest-tree metric d_j(X), Balaban uses sufficiently large tree-decay exponent to obtain a scale/volume-uniform anchored sum over localization domains containing a fixed pi_j cube. This is the direct source for C_geo(lambda) used in YM-SOURCE-GEOMETRIC-SUM-1.
- CMP116, "Renormalization group approach to lattice gauge field theories. II. Cluster expansions", Lemma 3 / Eq. (2.38): the exponentiated connected cluster activity has exponential tree decay with a reserve in the exponent; the following source argument states these estimates satisfy the convergence conditions for the cluster expansion. This independently supports the local connected-activity Banach topology.
- CMP122-II, after Eq. (1.89): for an arbitrary large-field region the local T'_k bound is improved by an additional exponential tree-distance factor. Eqs. (1.90)-(1.100) then decompose the R-operation result into connected localization polymers, prove tree decay, exponentiate the polymer expansion, and obtain the regular-sector bound exp[-p_0(g_k)] exp[-kappa d_k(X)].

Iteration 6 interpretation:
The source architecture is local/connected. Global absolute summability over COMPLETE histories is not a source requirement and is mathematically too strong. The correct reconstruction target is an anchored connected relative log-density/activity norm. The remaining issue is a uniform Lipschitz map from the complete compressed outer-history/T representation into one fixed connected-activity Banach chart.


Iteration 7 source/C1 audit:
- CMP119 Sect.2 source records: E^(j) and B^(j) local terms have analytic extensions on multiscale domains U_j^c(X,alpha_0,j,alpha_1,j); alpha_0,j and alpha_1,j scale like g_j times possibly different powers of log(g_j^-2). This confirms that a naive single scalar field rescaling is not automatically a uniform chart.
- CMP119 paragraph before Theorem 1: newly generated E^(k),R^(k),B^(k) terms are on slightly larger analytic spaces and have improved decay, with source examples such as kappa -> (1+4 beta)kappa. This is qualitative source evidence for an analytic/decay reserve; exact enlarged-domain coefficients remain unextracted and are not used as a numerical delta_ref.
- CMP116 support/locality extraction: localized cluster terms are built from restricted fields on a connected component plus source-specific base-cube/collar enlargement. The exact support-enlargement dictionary is still an explicit open obligation; this is precisely the missing r_col input for YM-CLUSTER-JACOBIAN-SCHUR-1.
- CMP122-II (1.97)-(1.99): the R' exponentiation step visibly changes the tree exponent from (1+beta)kappa for the connected pre-exponentiation activity to (1+beta/2)kappa for the post-exponentiation local log-activity. Iteration 7 records this as the source-visible decay loss beta*kappa/2.
- CMP122-II (1.99) also records an analytic extension of the localized R' term in the source field/source variables. This is not by itself analyticity of the map with respect to arbitrary anchored input activity coefficients; the latter is obtained only after a strict cluster-expansion convergence-ball theorem is identified.


Iteration 8 — direct-primary source ledger for regular R' C^1:

CMP122-II, printed pp.388-390:
- Eq. (1.90): after decomposition into connected components X'_p and resummation at fixed components, the R-operation factor has a hard-core polymer expansion 1+sum' product_p F(X'_p).
- Eq. (1.91): F(X') is obtained by resumming the source expressions determining the connected localization domain X'.
- Eq. (1.97): connected pre-exponentiation activity bound
  |F(X')| <= c_1 exp[-(1+beta)kappa d_{k,Ycal}(X')],
  where c_1=exp[-p_0(g_k)] if X' is disjoint from Ycal=union_iY_i, and c_1=alpha_LF^(1/3) in the boundary-intersecting lane.
- The source text around (1.97) gives explicit sufficient smallness inequalities for alpha_LF^(1/3) and exp[-p_0(g_k)/2] in terms of beta,kappa and states that g_k is chosen sufficiently small for the displayed absorptions.
- Eq. (1.98): the convergent cluster expansion is exponentiated as exp R'^(k)=exp(sum_X R'^(k)(X)).
- Eq. (1.99): general post-log local activity decay uses the relative metric and exponent (1+beta/2)kappa.
- Eq. (1.100): the second/regular group, disjoint from the indicated large-field set, has c_1=exp[-p_0(g_k)] and ordinary d_k(X):
  |R'^(k)(X)| <= exp[-p_0(g_k)] exp[-kappa d_k(X)].
- The first group is assigned to boundary B' terms and is not included in YM-RPRIME-C1-1.

CMP122-II Eq. (1.67):
- d_{k,Z}(Y) is the shortest-tree size measuring components of Y\Z. Therefore for Y disjoint from Z, d_{k,Z}(Y)=d_k(Y). This is used to turn the regular pre-exponentiation (1.97) bound into an ordinary d_k anchored input estimate.

CMP119 introduction:
- p_0(g) has the source polylogarithmic form A_0(log g^{-2})^{p_*} with positive p_*. Hence p_0(g)->infinity as g->0. This is used only to choose a fixed small-coupling threshold placing the actual source activity strictly inside the explicit KP ball.

Iteration-8 derived theorem discipline:
- The explicit activity convergence radius
  r_*=a exp[-16a-2theta]
  is NOT quoted as Balaban's hidden threshold. It is a new sufficient radius derived from generalized Kotecky-Preiss plus the already-proved d=4 tree coercivity.
- The regular cluster/log bounded C^1 theorem uses only this explicit ball and the primary source input bound.
- The stronger quasi-local Schur estimate additionally assumes theta-mu_out-nu remains in the CMP109 geometric-summability regime; no numerical Balaban kappa threshold is inferred from qualitative "sufficiently large" language.


Iteration 9 — exact (1.90)/(1.98) audit:

DIRECT PRIMARY CMP122-II printed pp.387-388:
- Before (1.90), each raw term determines X'_0 = (union_{Y in D}Y) union (union_j X_j).
- Balaban defines a special component decomposition: first components of X'_0\Ycal are taken; two are considered connected if they are contained in one localization domain Y of a term in the product.
- He then writes X'_0=union_p X'_p and states that the expression factorizes in these components.
- Resumming expressions determining the same components yields the primed polymer expansion (1.90).
- Consequence verified here: members of each actual primed family have pairwise disjoint pi_k-cube carriers.
- Missing source certificate: the displayed passage does not define the prime by an iff saying that every pairwise carrier-disjoint family is admissible. Therefore native incompatibility is not yet identified exactly with carrier overlap.

DIRECT PRIMARY CMP109 printed p.257:
- localization domains are unions of wall-connected pi_j cubes;
- connectivity is by common (d-1)-walls;
- d_j(X) is a shortest continuous-space tree meeting all cubes after unit-cube rescaling.
This is why Iteration 9 uses cube carriers rather than naive intersection of closed continuous cube unions.

DIRECT PRIMARY CMP122-II printed p.390:
- (1.98) gives exp R'^(k)=exp sum_X R'^(k)(X);
- the following text says native R'^(k)(X) depends on U_k restricted to X, is analytically extendible, and is given by the convergent series (7.13) [I] with notational changes;
- (1.99) gives its local bound;
- the following split defines boundary/first-group and regular/second-group terms, with (1.100) for the regular group.

REFERENCE CHECK:
- CMP122-II reference [I] is Balaban CMP109 (1987).
- The accessible published CMP109 PDF does not expose the cited formula under numbering (7.13); searches for 7.13 and for an explicit prime-definition did not recover it.
- Therefore this notebook does not claim coordinatewise R'_Bal(X)=H_X^can merely from (1.98).

Iteration-9 provenance rule:
Canonical exact-union grouping is OUR explicit chart for the (1.90) polymer gas. Its derivative-support theorem is rigorous in that chart. Native Balaban coordinate identification remains a separate source obligation.


Iteration 9 FINAL correction — resolving the legacy cluster references:

CMP122-II bibliography:
- [I] = CMP109 (1987), "Renormalization group approach ... I";
- [II] = CMP116 (1988), "Renormalization group approach ... II. Cluster expansions".

The printed CMP122-II text nevertheless cites legacy formulas "(7.1) [I]" and "(7.13) [I]".
The published CMP109 version ends before the required cluster-localization construction and explicitly states that the localization and cluster expansion will be constructed in the next paper.

Published CMP116 is exactly that next cluster-expansion paper. Its Sect. 2 supplies the formula-level crosswalk:
- CMP116 (2.1): first Mayer-expansion step;
- CMP116 (2.11): polymer expansion and exact incompatibility function zeta;
- CMP116 (2.12): connected-graph/Ursell logarithm;
- CMP116 (2.13): local coefficient grouped by exact target union union_i Z_i=X.

CMP122-II p.387 independently says its first R-operation exponentiation step is the Mayer expansion "the same as in (7.1) [I]".
CMP122-II p.390 says native R'^(k)(X) is given by the convergent series "(7.13) [I]" with proper notational changes.

Together these two anchors resolve the legacy manuscript numbering as:
legacy (7.1)[I] <-> published CMP116 (2.1),
legacy (7.13)[I] <-> published CMP116 (2.13).

This source crosswalk is now used to identify native R'^(k)(X) with the exact-union connected coefficient, rather than inferring coordinate equality from equality of total logarithms.

Exact published compatibility from CMP116 (2.11):
zeta(Z,Z')=0 iff Z cap Z' contains a cube or a wall of a cube; zeta=1 otherwise.

Iteration-9 geometry derived from that source convention in d=4:
- incompatibility support = closed one-step wall-neighborhood of the cube carrier;
- C_inc=9;
- tree-stitch cost c_stitch=sqrt(7);
- corrected native sufficient KP radius
  r_*=(a/9)exp[-16a-sqrt(7)theta].

The earlier Iteration-8 radius a exp[-16a-2theta] is retained only as a theorem for literal cube-overlap compatibility and is not used as the native Balaban radius.
