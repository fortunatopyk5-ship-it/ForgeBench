# Balaban-native matching state space audit — Iteration 4

Scope: mathematical validity of the matching state space for YM-MATCH-EXTRACT-1 only. STEP_0 is intentionally out of scope.

## 1. Coefficient product is not yet a space of effective densities

Let Adm_k denote the exact set of Balaban-admissible domain histories at RG scale k. Its primary-source definition has NOT yet been directly verified in this run; therefore admissibility/nesting conditions are not invented here.

For each admissible history Sigma and local label i=(sector,j,X,z,...) let H_i be the Banach space of bounded holomorphic local functions on the source analytic domain U_i, with the H-infinity norm. Let d_i be the source localization metric. Define the pointwise coefficient space

B_pt(kappa_*) =
 {F=(F_i): sup_i exp(kappa_* d_i) ||F_i||_{H_i} < infinity}.

This is a Banach space, but an arbitrary F in B_pt need not reconstruct a legitimate Balaban density.

The source density has the schematic nonlinear form

rho_k = Rec_k(c,F)
      = sum_{Sigma in Adm_k}
          chi_Sigma T_Sigma [ exp A_Sigma(c,F) ].

Therefore the true state must include reconstruction, not only coefficients.

## 2. Linear compatibility layer

The following constraints are linear whenever they can be formulated with bounded linear maps on the local H_i spaces:

1. index only source-admissible histories;
2. support/locality: every local activity is intrinsically a function only of the source-restricted field variables;
3. gauge invariance/covariance;
4. Euclidean covariance when required;
5. normalization conditions such as F_i(1)=0;
6. any genuine source restriction/overlap consistency equation of the form
   R_ij F_i = R_ji F_j.

Admissibility and nesting are best encoded in the index set Adm_k rather than as equations on F.

Define C_lin as the intersection of all verified bounded linear compatibility kernels. If the family of compatibility operators C_a is bounded, then

C_lin = intersection_a ker C_a

is a closed linear subspace of B_pt and hence Banach.

This is YM-COMPAT-CLOSED-1 below.

Important: the exact cross-history restriction/overlap equalities required by Balaban have NOT been extracted from primary text. They must not be guessed. For now C_lin contains only those compatibility conditions whose source meaning has been verified.

## 3. Actual-density state is a nonlinear graph

Let D_k be a Banach space in which the reconstructed density is to live (for example an appropriate bounded local-density space once history summability is supplied).

Given a continuous reconstruction map

Rec_k : I_c x C_lin -> D_k,

define

M_adm,k =
 {(c,F,rho):
    c in I_c,
    F in C_lin,
    rho = Rec_k(c,F),
    rho >= 0,
    N(rho)=1}.

Because of exponentiation, sector integration T_Sigma, positivity and normalization, M_adm,k is generally NOT a linear subspace. It is a constrained nonlinear graph (or chart/manifold-like subset).

If Rec_k is continuous, the graph condition is closed. Positivity and a continuous normalization N(rho)=1 are closed constraints. Hence M_adm,k is closed in I_c x C_lin x D_k.

But continuity of Rec_k is not established until the sum over domain histories is uniformly controlled. Thus closedness of the actual-density state space is currently CONDITIONAL, not proved for Balaban.

## 4. Why l-infinity over histories is insufficient

For sector amplitudes a_Sigma >= 0,

sup_Sigma a_Sigma

does not control

sum_Sigma a_Sigma.

Counterexample: N histories with chi_Sigma=1 and sector contribution equal to one. The l-infinity norm is one while the reconstructed sum is N. If N grows with RG depth/cutoff, no cutoff-uniform reconstruction bound follows.

Therefore the old native pointwise norm is only a component norm. A second history/reconstruction norm is mandatory unless the source proves exact disjointness or a uniform finite-overlap theorem.

## 5. Three possible history mechanisms

Let a_Sigma(F) be a nonnegative majorant for the sector contribution before the history sum, including all local component norms needed in the sector.

### A. Exact disjointness

If for every field U

chi_Sigma(U) chi_Sigma'(U)=0
for Sigma != Sigma',

then at most one sector contributes and an l-infinity history norm is enough.

STATUS: NOT VERIFIED. The presence of characteristic functions or a small/large-field partition does not by itself prove this for complete multiscale histories.

### B. Uniform bounded overlap

If

sup_U # {Sigma in Adm_k: chi_Sigma(U) != 0} <= C_hist

with C_hist independent of k, cutoff and volume, then

sup_U sum_Sigma |chi_Sigma(U)| a_Sigma
 <= C_hist sup_Sigma a_Sigma

provided |chi_Sigma|<=1.

STATUS: NOT VERIFIED; no source-derived C_hist has been obtained.

### C. Weighted summability

The robust formulation is to supply source weights W_Sigma >= 0 and operator majorants tau_Sigma for T_Sigma such that

H_hist =
 sup_U sum_{Sigma in Adm_k}
 |chi_Sigma(U)| tau_Sigma / w_Sigma
 < infinity

uniformly in k/cutoff/volume,

while the sector data obey
a_Sigma(F) <= R / w_Sigma.

Then

||Rec_k(c,F)|| <= H_hist R

up to the explicitly controlled exponential/local normalization factors.

This is the natural replacement if histories overlap or branch combinatorially.

STATUS: FORMULATED BUT NO BALABAN HISTORY WEIGHT HAS BEEN EXTRACTED.

## 6. Proposed pre-state norm

Until a source theorem identifies A or B, the candidate state norm must include a history-variation seminorm. Schematically,

||F||_pre =
 max(
   ||F||_pt,
   sup_U sum_{Sigma in Adm_k}
      |chi_Sigma(U)| tau_Sigma a_Sigma(F)
 ).

This is meaningful only after tau_Sigma and the history contribution map are made precise. It is intentionally not declared to be the final Balaban Banach norm yet.

## 7. Native metric verdict

A source-audit entry pinned to Balaban CMP109, printed page 257 / PDF page 9, records the primary definition:

- a localization domain X is a union of a connected finite family of pi_j-cubes;
- d_j(X) is the length of a shortest tree graph contained in X and intersecting all cubes in X, divided by M.

Thus the metric is a shortest-tree/localization-size metric, not diameter.

Direct primary PDF rendering of this page was not available to this run, so this remains SOURCE-EXTRACTED VIA THE AUDIT LEDGER rather than newly direct-primary-verified.

### Why diameter would have failed

For a diameter-only metric in dimension d>=2, the number of connected subsets containing a fixed root and lying in a ball of diameter O(n) can grow like exp(c n^d), not exp(c n). Hence a claim

# {X contains B: diam(X) in [n,n+1)} <= C exp(c n)

is generally false.

The tree-size metric avoids this particular failure because tree length can control the number of cubes.

## 8. Counting theorem for the actual tree-size geometry

Let G be the adjacency graph of source blocks with maximum degree Delta<infinity. Suppose the source tree metric d(X) obeys the geometric coercivity estimate

|X| <= a_0 + a_1 d(X)                                         (TC)

for every connected source localization domain X.

Let A_Delta(m) be any exponential lattice-animal majorant,

# {connected X containing B: |X|=m}
 <= C_A exp(c_A m).

Then for n>=0,

# {X containing B: n <= d(X) < n+1}
 <=
 sum_{m <= a_0+a_1(n+1)} C_A exp(c_A m)
 <= C_count exp(c_count n)

with explicit symbolic choices depending only on
Delta,a_0,a_1,C_A,c_A.

Therefore exponential shell counting follows from:
(i) bounded lattice degree,
(ii) the tree metric,
(iii) TC.

Iteration-5 correction: this Balaban-specific item is now closed by direct CMP109 verification. The source shortest-tree definition gives
|X|<=16(d_j(X)+1)
in d=4, uniformly in j/volume/cutoff (YM-BALABAN-TREE-COERCIVITY-1). Combined with degree-8 cube adjacency and DFS counting, the fixed-history shell estimate is explicit (YM-BALABAN-TREE-SHELL-COUNT-1).

## 9. Histories cannot be counted from d_j(X) alone

Even a perfect bound on

# {X: n<=d_j(X)<n+1}

does not control

# {(Sigma,X): n<=d_Sigma(X)<n+1}

because arbitrarily many admissible histories can share the same local polymer X.

Hence polymer counting and history summability are logically distinct certificates.

The strong pair-counting bound requested in the prompt is NOT established. It requires a history multiplicity/weight theorem in addition to tree counting.

## 10. Common analytic-domain problem

CMP119 source transcriptions state that local analytic domains use parameters of the schematic form

alpha_{0,j} = g_j C_0 (log g_j^{-2})^{q_0},
alpha_{1,j} = g_j C_1 (log g_j^{-2})^{q_1}.

The complete effective action retains components generated at historical scales j.

Along the UV trajectory, early-scale g_j can approach zero as the cutoff is removed. Therefore the raw analytic radii alpha_{r,j} may approach zero.

Consequently a naive common raw domain

U_ref(X) subset U_{Sigma,j}^*(X)
for all historical j

need not have a positive cutoff-uniform analytic collar. In fact the available formulas make

delta_* = inf_{j,Sigma} analytic_radius(j,Sigma)

potentially zero.

Thus a common unscaled analytic core has NOT been established and may be the wrong construction.

### Correct repair candidate: normalized analytic charts

For each local source domain introduce a source-faithful chart

Psi_{Sigma,j,X}: U_ref(X) -> U_{Sigma,j}^*(X)

that rescales the small-field coordinates by their native alpha_{0,j},alpha_{1,j}. Define

F_tilde_{Sigma,j,X} = F_{Sigma,j,X} o Psi_{Sigma,j,X}.

A fixed reference H-infinity space is obtained only if one proves uniform bounds on:
- Psi and its inverse on the needed core;
- gauge covariance;
- compatibility with restriction/localization;
- RG chart transitions;
- a positive reference Cauchy collar delta_ref.

No such uniform chart theorem has been extracted yet.

## 11. Localized coupling profile: source-derived, not an independent physical coordinate

Primary CMP119 Eq. (2.24) defines

c_{j-1}(x)=c_j(x)+beta_j(g_{j-1})phi_j(x),
where c_j(x)=1/g_j^2(x),

with phi_j in C_0^infinity(Lambda_j) and phi_j=1 on the prescribed shrunken interior.

Thus a scalar bulk coupling alone does not reproduce the pointwise coefficient, but the source also does not sum or integrate over phi_j. It is localization auxiliary data attached to Lambda_j.

After fixing a deterministic cutoff-selection convention Lambda -> phi_Lambda, the full profile is DERIVED from the bulk coupling sequence and the domain history. It should not be an arbitrary physical q-coordinate.

On the interior where phi_j=1 the source-native direction is

c_j=c_{j-1}-beta_j(g_{j-1}).

The old ell_W coordinate remains suspended.

## 12. Compatibility conditions: current exact status

| Condition | Mathematical form | Closed/linear? | Source status |
|---|---|---|---|
| outer histories | H_k=(Omega_j,Lambda_j) generated by CMP119 construction | index restriction | DIRECT PRIMARY core + recursive construction verified |
| nesting | Omega_1 superset Lambda_1 superset ... superset Omega_k superset Lambda_k | index restriction | DIRECT PRIMARY Eq. (2.1) |
| internal S-history | S_j subset Omega_j intersect Lambda_j^c with source cube/collar rules | internal T index | DIRECT PRIMARY |
| raw P/Q/R histories | exact hard partitions, then resummed/compressed | auxiliary indices | DIRECT PRIMARY |
| local support | F_i intrinsically on source local variables | linear | source-local structure verified componentwise |
| gauge invariance | F_i(g.U)=F_i(U) | closed linear | source verified for relevant local analytic pieces |
| Euclidean covariance | pullback equalities | closed linear | source verified for indicated components |
| normalization/subtractions | component-specific source convention | affine/linear | source verified componentwise, global dictionary incomplete |
| density reconstruction | rho=Rec(c,F) with T/zeta history kernels | nonlinear | source formula verified; fixed Banach continuity still open |
| positivity/normalization of rho | rho>=0, N(rho)=1 | nonlinear/affine | requires chosen density topology |

The complete admissible-history notion is best defined recursively/generatively, not by inventing additional cross-history overlap identities.

## 13. RG preservation

DIRECT PRIMARY CMP122-II Theorem 1 and the end of its construction prove that, under its small-coupling hypotheses, the result of the R-operation is again writable in CMP119 Eq. (2.18) form and satisfies the Sect. 2 induction hypotheses.

This closes semantic preservation of the SOURCE inductive class.

What is not proved is the stronger functional-analytic statement

R: U subset B_amb -> B_amb

on one fixed open Banach neighborhood with C^1/C^2 control.

To obtain that statement we still need:
1. normalized fixed analytic charts across historical scales;
2. fixed history weights translating source entropy/energy estimates into B_amb;
3. a volume-independent local/relative density target for reconstruction;
4. operator bounds for the full source RG in those fixed charts.

Thus source-class preservation is DIRECT PRIMARY VERIFIED; topology-level open-neighborhood preservation remains BLOCKED.

## 14. Reconstruction lemmas

### YM-HISTORY-WEIGHT-1
For sector maps with
||Sec_H||<=tau_H a_H,
a_H<=R/w_H,
and
H_hist=sup_U sum_H |chi_H(U)| tau_H/w_H<infinity,

one gets the corresponding weighted sum bound. This abstract lemma remains useful when the history kernel genuinely factors into chi_H times Sec_H.

For CMP119 Eq. (2.18), however, there is no source chi_H for the complete compressed history. The practical source kernel is the whole T/zeta/history operator.

### YM-HISTORY-RECON-ABSTRACT-1
For compressed-history linear kernels K_H satisfying
||K_H G||<=tau_H||G||,
the weighted ambient space
||A||_w=sup_H w_H||A_H||
has absolute/Lipschitz reconstruction on radius R whenever

H_0=sum_H tau_H<infinity,
H_1=sum_H tau_H/w_H<infinity.

Then

||Rec(A)||<=e^R H_0,

||Rec(A)-Rec(B)||<=e^R H_1||A-B||_w.

CMP122-II now directly verifies where the source tau_H suppression comes from, but fixed H_0,H_1 in our chosen topology remain to be extracted.

## 15. K_0 decision

A mathematically legitimate quantitative matching tube still requires:

[c_-,c_+],
fixed normalized analytic charts,
kappa_* and component radii,
a volume-independent local/relative density target D,
fixed history weights/operator majorants yielding finite H_0,H_1,
and an open-neighborhood native RG theorem in B_amb.

Fixed-history polymer/tree constants are no longer a blocker: they are explicit after the direct CMP109 audit.

An independent physical coupling-profile radius is removed from this list: the profile is history-derived once the cutoff convention is fixed.

These data are not all available.

Therefore:

STATUS: BLOCKED AT YM-HISTORY-RECON-1.

K_0 is NOT constructed, and STEP_0 remains out of scope.

# Iteration 5 correction — history compression, T-summation, and physical coupling profile

## 18. "History" is representation-dependent

Direct primary CMP119 verification shows that a single symbol Sigma obscures three levels:

1. raw partition history: temporary P_j,Q_j,R_j,... choices from hard decompositions of unity;
2. compressed outer history H_k=(Omega_1,Lambda_1;...;Omega_k,Lambda_k) appearing in Eq. (2.18);
3. internal S-history (S_1,...,S_k), whose sum is explicitly included inside T_k.

Raw P,Q choices producing the same Omega are resummed into zeta(Omega^c). Thus the physical reconstruction topology should be built for the COMPRESSED representation only after the internal T/zeta summability has been included in the operator weight.

## 19. Correct characteristic verdict

The elementary raw decompositions (CMP119 3.2,3.3,3.16) are exact {0,1} partitions of unity.

However Eq. (2.17) chi_k(Omega_k) depends only on the terminal small-field region, and Eq. (2.18) still sums over full outer histories while S is hidden in T_k. Therefore

sum_{H in Adm_k^out} chi_k(Omega_k(H)) = 1

is NOT a source identity and must not be used.

For the compressed state, history control belongs to the T/zeta weights and large-field energy factors.

## 20. Direct source answer to history entropy

CMP122-II directly estimates the summations over admissible sequences by

exp{O(1)(MR_j)^(-d)|Z_j|}

before Eq. (1.79). The same estimate carries large-field penalties generated by the raw characteristic functions. Their source component form contains p_0(g_j)^2 tree/linear-size suppression and -2p_0(g_j) terms. The subsequent induction proves a residual cost kappa_j(Z), with a new exp[-p_0(g_j)] whenever a new large field is created.

Therefore the source convergence mechanism is:

raw exact partition
 -> compression/resummation
 -> explicit admissible-sequence entropy factor
 -> large-field energy domination.

The unresolved problem is not whether Balaban pays history entropy; he does. The unresolved problem is transporting that multiscale estimate to one fixed Banach norm.

## 21. q-profile correction

Iteration 4 treated q(x)=c(x)-c as if it had to be a separate physical coordinate.

Primary CMP119 Eq. (2.24) changes this conclusion. The source defines

1/g_{j-1}^2(x)=1/g_j^2(x)+beta_j(g_{j-1})phi_j(x),

where phi_j is a localization cutoff supported in Lambda_j and equal to one on the prescribed inner part.

phi_j is not summed/integrated as a state variable.

Hence, once a deterministic cutoff-selection convention Lambda->phi_Lambda is fixed, the profile is DERIVED from the domain history and bulk coupling sequence.

The physical state should not contain an arbitrary independent q.

An auxiliary freely variable q may be introduced only as an off-physical ambient extension for Frechet calculus, and only after proving the native RG extends to such variations.

## 22. Ambient versus physical space

For future Taylor analysis the ambient coefficient space at fixed k is taken abstractly as

B_amb^(k)(w)
 =
 C_c direct_sum l^infinity_w({A_H}_{H in Adm_k^out}),

where A_H is a fixed-reference-chart Banach algebra of action perturbations and

||(delta c,A)||=|delta c|+sup_H w_H||A_H||.

This is a genuine Banach space with open balls.

The physical subset M_phys^(k) imposes:
- source history compatibility/admissibility;
- history-derived localized coupling profile;
- reconstruction by the source T/zeta kernels;
- positivity and normalization.

Thus M_phys is constrained/nonlinear but sits inside a linear ambient space.

Remaining blocker:
the normalized analytic reference charts and fixed history weights w_H are not yet source-instantiated, and the RG has not been proved to extend as a C^1/C^2 map on an open neighborhood in this B_amb.

## 23. Reconstruction theorem status

YM-HISTORY-RECON-ABSTRACT-1 gives a complete abstract sufficient criterion.

If compressed-history kernels K_H satisfy
||K_H G||_D<=tau_H||G||,
and for weights w_H>=1

H_0=sum_H tau_H<infinity,
H_1=sum_H tau_H/w_H<infinity,

then on the radius-R ambient ball,

||Rec(A)||_D<=e^R H_0,

||Rec(A)-Rec(B)||_D<=e^R H_1||A-B||_amb.

CMP122-II supplies the source entropy/energy mechanism and local T majorants, but H_0,H_1 have not yet been extracted in one volume-independent local/relative D.

Consequently:

STATUS: BLOCKED AT YM-HISTORY-RECON-1.


# Iteration 6 correction — ambient history norm is local/connected, not global-history l-infinity reconstruction

## 24. Global complete histories are not Banach coordinates for local physics

The complete-history coefficient product remains useful as source bookkeeping, but it is NOT the final reconstruction norm. Global history multiplicity can grow exponentially with volume even for exact independent partitions.

The physical/local Banach coordinates after source resummation are connected localization activities/operators.

## 25. Preferred ambient local activity space

For each connected source localization domain X use the fixed-reference analytic Banach space A_X and define

K_{loc,mu}
=
{K: sup_Delta sum_{X contains Delta} e^{mu d(X)}||K_X||_{A_X}<infinity}.

The ambient crossover state should eventually be a product of:
- a bulk marginal coupling coordinate;
- E/R/B connected local activity spaces;
- connected T/operator activity space;
- optional marked/source activity spaces.

Vacuum energy density is a separate scalar; total extensive vacuum energy is not part of the norm.

This space has open balls and is appropriate for future Frechet analysis once the fixed analytic charts have been constructed.

## 26. Physical subset

The physical source subset consists of activity families generated by Balaban's recursive hard partitions, resummations, T-operations and R-operation, satisfying positivity/normalization in the reconstructed density.

Remote disconnected history choices are not separate local coordinates after connected resummation.

## 27. Revised history norm blocker

CMP109 (0.26) plus CMP122-II local tree-decay estimates establish finite anchored local H_0 bounds for important T/R history sectors.

The remaining state-space problem is the UNIFORM LIPSCHITZ MAP

pre-resummed compressed outer-history data
   -> connected local activity/operator coordinates

in one fixed normalized analytic chart.

Thus K_0 remains prohibited/unconstructed.


# Iteration 7 correction — C^1 ambient structure

The current ambient history/activity space is no longer described by one untyped global family. Use explicit anchored input/output spaces with scale/polymer coordinate Banach spaces.

A fixed coefficient Banach space may be SCALE-GRADED:
coordinates at historical scale j may live in their native H^infinity(U_j^c) spaces. YM-GRADED-ACTIVITY-BANACH-1 shows that one fixed anchored product remains Banach even when the coordinate spaces differ.

Therefore:
- common normalized raw field domain is NOT required merely for coefficient-space C^1;
- bounded restriction/localization maps between the graded coordinate spaces are required;
- a uniform normalized field/source collar remains required for actual U/J source derivatives and future chart-homogeneous RG iteration.

The full history map will be considered C^1 only after either:
(1) the weighted Jacobian Schur bound is source-instantiated, or
(2) a uniform larger activity-ball analytic bound is proved and Banach Cauchy applies.

No K_0 follows from these observations.
